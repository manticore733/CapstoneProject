import { CreateEmployeeDto } from "./CreateEmployeeDto";

// Your UpdateEmployeeDto allows partial updates
export type UpdateEmployeeDto = Partial<CreateEmployeeDto>;