import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ClientUser } from './client-user';

describe('ClientUser', () => {
  let component: ClientUser;
  let fixture: ComponentFixture<ClientUser>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ClientUser]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ClientUser);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
