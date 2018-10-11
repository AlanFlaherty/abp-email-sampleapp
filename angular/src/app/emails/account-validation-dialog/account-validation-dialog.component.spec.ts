import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountValidationDialogComponent } from './account-validation-dialog.component';

describe('AccountValidationDialogComponent', () => {
  let component: AccountValidationDialogComponent;
  let fixture: ComponentFixture<AccountValidationDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AccountValidationDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AccountValidationDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
