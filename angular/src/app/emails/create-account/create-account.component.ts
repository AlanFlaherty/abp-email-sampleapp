import { Component, OnInit, ViewChild } from '@angular/core';
import { AccountValidationDialogComponent } from '@app/emails/account-validation-dialog/account-validation-dialog.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';

// TODO: remove before checkin
import { inspect } from 'util';
import { ValidateAccountInput } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-create-account',
  templateUrl: './create-account.component.html',
  styleUrls: ['./create-account.component.css'],
  animations: [appModuleAnimation()]
})
export class CreateAccountComponent implements OnInit {

  @ViewChild('accountValidationDialog') accountValidationDialog: AccountValidationDialogComponent;

  emailSettings: EmailSettingsDto;


  viewModel: CreateAccountViewModel;
  accountValidated: boolean = false;
  

  constructor() { }

  ngOnInit() {
    this.emailSettings = new EmailSettingsDto();
    this.viewModel = new CreateAccountViewModel();
  }

  // TODO: implement
  l(name:string) : string{
    return name;
  }

  validateAccountDetails(): void{
    this.accountValidationDialog.show(this.viewModel,
      (valid: boolean)=>{
        this.accountValidated = valid;
      }
    );
  }

  inspect(object: any):string{
    return inspect(object);
  }

  saveAccountDetails(): void {

  }
}

export class CreateAccountViewModel {
  emailAddress: string;
  userName: string;
  password: string;
  storePassword: boolean;

  imapServer: string;
  imapPort: number;
  imapSecurityType: string; // TODO: Convert to enum

  smtpServer: string;
  smtpPort: number;
  smtpSecurityType: string;
  
  toValidateAccountInput(sourceComponentGuid: string, signalRConnectionID: string) : ValidateAccountInput{
    return new ValidateAccountInput({
      'sourceComponentGuid': sourceComponentGuid, 
      'signalRConnectionID': signalRConnectionID,
      'username': this.userName,
      'password': this.password,
      'iMapServerName': this.imapServer,
      'iMapServerPort': this.imapPort,
      'iMapSecurityType': this.imapSecurityType,
      'smtpServerName': this.smtpServer,
      'smtpServerPort': this.smtpPort,
      'smtpSecurityType': this.smtpSecurityType
    });
  }
}

export class EmailSettingsDto { 
  userId: number; 
  userName: string; 
  password: string; 
  isPasswordStored: boolean; 
  iMapServer: string; 
  iMapPort: number; 
  iMapSecurityType: string; 
  smtpServer: string; 
  smtpPort: number; 
  smtpSecurityType: string; 
  id: number;
  constructor(data?: any) {
      if (data !== undefined) {
          this.userId = data["userId"] !== undefined ? data["userId"] : null;
          this.userName = data["userName"] !== undefined ? data["userName"] : null;
          this.password = data["password"] !== undefined ? data["password"] : null;
          this.isPasswordStored = data["isPasswordStored"] !== undefined ? data["isPasswordStored"] : null;
          this.iMapServer = data["iMapServer"] !== undefined ? data["iMapServer"] : null;
          this.iMapPort = data["iMapPort"] !== undefined ? data["iMapPort"] : null;
          this.iMapSecurityType = data["iMapSecurityType"] !== undefined ? data["iMapSecurityType"] : null;
          this.smtpServer = data["smtpServer"] !== undefined ? data["smtpServer"] : null;
          this.smtpPort = data["smtpPort"] !== undefined ? data["smtpPort"] : null;
          this.smtpSecurityType = data["smtpSecurityType"] !== undefined ? data["smtpSecurityType"] : null;
          this.id = data["id"] !== undefined ? data["id"] : null;
      }
  }
}