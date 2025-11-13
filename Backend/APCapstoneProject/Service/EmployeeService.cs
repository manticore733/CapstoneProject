using APCapstoneProject.DTO.Employee;
using APCapstoneProject.Model;
using APCapstoneProject.Repository;
using AutoMapper;
using ClosedXML.Excel;

namespace APCapstoneProject.Service
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repository;
        private readonly IMapper _mapper;
        private readonly IClientUserRepository _clientUserRepository;

        public EmployeeService(
             IEmployeeRepository repository,
             IClientUserRepository clientUserRepository,
             IMapper mapper)
        {
            _repository = repository;
            _clientUserRepository = clientUserRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EmployeeReadDto>> GetEmployeesByClientIdAsync(int clientUserId)
        {
            var employees = await _repository.GetByClientIdAsync(clientUserId);
            return _mapper.Map<IEnumerable<EmployeeReadDto>>(employees);
        }

        public async Task<EmployeeReadDto?> GetEmployeeByIdAsync(int id, int clientUserId)
        {
            var employee = await _repository.GetByIdAndClientIdAsync(id, clientUserId);
            if (employee == null) return null;
            return _mapper.Map<EmployeeReadDto>(employee);
        }

        public async Task<EmployeeReadDto> CreateEmployeeAsync(CreateEmployeeDto employeeDto, int clientUserId)
        {
            // --- 1. Ensure valid client ---
            var isClientUser = await _clientUserRepository.IsActiveClientUserAsync(clientUserId);
            if (!isClientUser)
                throw new KeyNotFoundException($"No active ClientUser found with ID '{clientUserId}'.");

            // --- 2. Fetch all existing employees for this client ---
            var existingEmployees = (await _repository.GetByClientIdAsync(clientUserId)).ToList();

            // --- 3. Check for duplicates (same email or account) ---
            bool duplicateExists = existingEmployees.Any(e =>
                e.Email.Equals(employeeDto.Email, StringComparison.OrdinalIgnoreCase) ||
                e.AccountNumber.Equals(employeeDto.AccountNumber, StringComparison.OrdinalIgnoreCase));

            if (duplicateExists)
                throw new InvalidOperationException($"An employee with email '{employeeDto.Email}' or account '{employeeDto.AccountNumber}' already exists.");

            // --- 4. Validate salary and date ---
            if (employeeDto.Salary <= 0)
                throw new ArgumentException("Salary must be greater than zero.");

            if (employeeDto.DateOfJoining == default)
                throw new ArgumentException("Date of Joining is required.");

            // --- 5. Create new employee ---
            var employee = _mapper.Map<Employee>(employeeDto);
            employee.ClientUserId = clientUserId;
            employee.IsActive = true;
            employee.CreatedAt = DateTime.UtcNow;

            await _repository.AddAsync(employee);
            return _mapper.Map<EmployeeReadDto>(employee);
        }





        public async Task<bool> UpdateEmployeeAsync(int id, UpdateEmployeeDto employeeDto, int clientUserId)
        {
            var isClientUser = await _clientUserRepository.IsActiveClientUserAsync(clientUserId);
            if (!isClientUser)
            {
                throw new KeyNotFoundException($"No active ClientUser found with ID '{clientUserId}'.");
            }

            // Get the employee  check ownership
            var existing = await _repository.GetByIdAndClientIdAsync(id, clientUserId);
            if (existing == null)
            {
                return false; // Not found or doesn't belong to this user
            }

            // Map the DTO onto the existing model
            _mapper.Map(employeeDto, existing);

            await _repository.Update(existing);
            return true;

        }

        public async Task<bool> DeleteEmployeeAsync(int id, int clientUserId)
        {
            var isClientUser = await _clientUserRepository.IsActiveClientUserAsync(clientUserId);
            if (!isClientUser)
            {
                throw new KeyNotFoundException($"No active ClientUser found with ID '{clientUserId}'.");
            }

            // Check ownership before deleting
            var existing = await _repository.GetByIdAndClientIdAsync(id, clientUserId);
            if (existing == null)
            {
                return false; // Not found or doesn't belong to this user
            }

            return await _repository.DeleteAsync(id);
        }


        public async Task<EmployeeUploadResultDto> ProcessEmployeeExcelAsync(IFormFile file, int clientUserId)
        {
            var result = new EmployeeUploadResultDto();

            // Load the Excel file
            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheet(1);

            var rows = worksheet.RowsUsed().Skip(1); // Skip header
            int rowIndex = 2;

            // 🔹 Fetch existing employees once 
            var existingEmployees = (await _repository.GetByClientIdAsync(clientUserId)).ToList();

            //  Keep track of duplicates within the uploaded Excel itself
            var seenEmails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var seenAccounts = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var row in rows)
            {
                try
                {
                    // --- Extract fields safely ---
                    string name = row.Cell(1).GetValue<string>().Trim();
                    string email = row.Cell(2).GetValue<string>().Trim();
                    string accountNumber = row.Cell(3).GetValue<string>().Trim();
                    string bankName = row.Cell(4).GetValue<string>().Trim();
                    string ifsc = row.Cell(5).GetValue<string>().Trim();
                    string salaryStr = row.Cell(6).GetValue<string>().Trim();
                    string dateOfJoiningStr = row.Cell(7).GetValue<string>().Trim();
                    string dateOfLeavingStr = row.Cell(8).GetValue<string>().Trim();

                    // --- Validate required fields ---
                    if (string.IsNullOrWhiteSpace(name))
                        throw new Exception("Employee name is required.");
                    if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
                        throw new Exception("Valid email is required.");
                    if (string.IsNullOrWhiteSpace(accountNumber))
                        throw new Exception("Account number is required.");
                    if (string.IsNullOrWhiteSpace(bankName))
                        throw new Exception("Bank name is required.");
                    if (string.IsNullOrWhiteSpace(ifsc))
                        throw new Exception("IFSC code is required.");

                    if (!decimal.TryParse(salaryStr, out decimal salary) || salary <= 0)
                        throw new Exception("Salary must be a positive decimal value.");

                    if (!DateTime.TryParse(dateOfJoiningStr, out DateTime dateOfJoining))
                        throw new Exception("Date of Joining is required and must be a valid date.");

                    DateTime? dateOfLeaving = null;
                    if (!string.IsNullOrEmpty(dateOfLeavingStr))
                    {
                        if (DateTime.TryParse(dateOfLeavingStr, out DateTime parsedLeaveDate))
                            dateOfLeaving = parsedLeaveDate;
                        else
                            throw new Exception("Invalid Date of Leaving format.");
                    }

                    // --- Check for duplicate within the same Excel file ---
                    if (!seenEmails.Add(email))
                        throw new Exception($"Duplicate email '{email}' found in Excel file.");
                    if (!seenAccounts.Add(accountNumber))
                        throw new Exception($"Duplicate account number '{accountNumber}' found in Excel file.");

                    // --- Check for duplicate in database for the same client ---
                    bool duplicateInDb = existingEmployees.Any(e =>
                        e.Email.Equals(email, StringComparison.OrdinalIgnoreCase) ||
                        e.AccountNumber.Equals(accountNumber, StringComparison.OrdinalIgnoreCase));

                    if (duplicateInDb)
                        throw new Exception($"Employee with email '{email}' or account '{accountNumber}' already exists in your records.");

                    // --- Create and save employee ---
                    var employee = new Employee
                    {
                        EmployeeName = name,
                        Email = email,
                        AccountNumber = accountNumber,
                        BankName = bankName,
                        IFSC = ifsc,
                        Salary = salary,
                        DateOfJoining = dateOfJoining,
                        DateOfLeaving = dateOfLeaving,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        ClientUserId = clientUserId
                    };

                    await _repository.AddAsync(employee);
                    existingEmployees.Add(employee); // add to in-memory list for next checks
                    result.SuccessCount++;
                }
                catch (Exception ex)
                {
                    result.FailedCount++;
                    result.FailedRows.Add(new FailedRowInfo
                    {
                        RowNumber = rowIndex,
                        ErrorMessage = ex.Message
                    });
                }

                rowIndex++;
            }

            return result;
        }


    }
}