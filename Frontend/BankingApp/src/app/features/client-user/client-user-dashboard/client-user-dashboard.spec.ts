import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ClientUserDashboard } from './client-user-dashboard';

describe('ClientUserDashboard', () => {
  let component: ClientUserDashboard;
  let fixture: ComponentFixture<ClientUserDashboard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ClientUserDashboard]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ClientUserDashboard);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
