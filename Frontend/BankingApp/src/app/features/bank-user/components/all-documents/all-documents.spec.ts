import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AllDocuments } from './all-documents';

describe('AllDocuments', () => {
  let component: AllDocuments;
  let fixture: ComponentFixture<AllDocuments>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AllDocuments]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AllDocuments);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
