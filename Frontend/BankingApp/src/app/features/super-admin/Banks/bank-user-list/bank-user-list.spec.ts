import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BankUserList } from './bank-user-list';

describe('BankUserList', () => {
  let component: BankUserList;
  let fixture: ComponentFixture<BankUserList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BankUserList]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BankUserList);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
