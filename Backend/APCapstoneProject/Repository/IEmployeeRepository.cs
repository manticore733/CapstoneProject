﻿using APCapstoneProject.Model;

namespace APCapstoneProject.Repository
{
   
        public interface IEmployeeRepository
        {
            Task<IEnumerable<Employee>> GetAllAsync();
            Task<Employee?> GetByIdAsync(int id);
            Task<Employee> AddAsync(Employee employee);
            Task<Employee> UpdateAsync(Employee employee);
            Task<bool> SoftDeleteAsync(int id);
        }
    
}
