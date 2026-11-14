import { CreateBeneficiaryDto } from "./CreateBeneficiaryDto";

// Your UpdateBeneficiaryDto allows partial updates
export type UpdateBeneficiaryDto = Partial<CreateBeneficiaryDto>;