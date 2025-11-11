import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BankUserDashboard } from './bank-user-dashboard';

describe('BankUserDashboard', () => {
  let component: BankUserDashboard;
  let fixture: ComponentFixture<BankUserDashboard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BankUserDashboard]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BankUserDashboard);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
