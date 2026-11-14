import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SuperAdminReports } from './super-admin-reports';

describe('SuperAdminReports', () => {
  let component: SuperAdminReports;
  let fixture: ComponentFixture<SuperAdminReports>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SuperAdminReports]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SuperAdminReports);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
