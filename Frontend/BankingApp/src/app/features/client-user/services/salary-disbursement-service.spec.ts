import { TestBed } from '@angular/core/testing';

import { SalaryDisbursementService } from './salary-disbursement-service';

describe('SalaryDisbursementService', () => {
  let service: SalaryDisbursementService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SalaryDisbursementService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
