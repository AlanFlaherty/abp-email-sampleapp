import { Component, OnInit, ViewChild, Output, ElementRef, EventEmitter, Injector } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { CreateAccountViewModel } from '@app/emails/create-account/create-account.component';
import { AccountValidationServiceServiceProxy, ValidateAccountInput } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/app-component-base';

import { HubConnectionBuilder, HubConnection } from '@aspnet/signalr';
import { Guid } from '@shared/helpers/Guid';
import { AppConsts } from '@shared/AppConsts';

type ValidationCallback = (valid: boolean) => any;

@Component({
  selector: 'account-validation-dialog',
  templateUrl: './account-validation-dialog.component.html',
  styleUrls: ['./account-validation-dialog.component.css']
})
export class AccountValidationDialogComponent extends AppComponentBase implements OnInit {

  @ViewChild('accountValidationModal') modal: ModalDirective;
  @ViewChild('modalContent') modalContent: ElementRef;

  @ViewChild('messageContainer') messageContainer: ElementRef;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active: boolean = false;
  saving: boolean = false;

  messages: string = "";
  percent: string = "0%";
  status: string = "Success";

  constructor(
    injector: Injector,
    private _accountValidationService: AccountValidationServiceServiceProxy
  ) {
      super(injector);
  }

  private connection : HubConnection;

  ngOnInit() {
    this.connection = new HubConnectionBuilder()
        .withUrl(AppConsts.remoteServiceBaseUrl + "/mail")
        .build();

    this.connection.on("ReceiveServiceProgressMessage", (sourceComponentGuid,  message, percentageComplete, status) => {
        console.log("ReceiveServiceProgressMessage");
        console.log("%s %s %d %s", sourceComponentGuid,  message, percentageComplete, status);

        this.messages += (message + "\r\n");
        this.messageNumber++;
        this.percent = percentageComplete + "%";
        this.status = status;
        this.scrollContentToBottom();
    });

    this.connection.start();
  }

  scrollContentToBottom(): void {
    this.messageContainer.nativeElement.scrollTop = this.messageContainer.nativeElement.scrollHeight;  }

  show(model:CreateAccountViewModel, callback: ValidationCallback): void {
    
    // reset
    this.status = "";
    this.messages = "";
    this.percent = "0%";
    this.status = "Success";

    this.active = true;
    alert(JSON.stringify(model));

    let input : ValidateAccountInput = model.toValidateAccountInput(Guid.newGuid(), "");
    alert(JSON.stringify(input));
    /*
    let input: ValidateAccountInput = new ValidateAccountInput();
    input.
    */

      // abp.ui.block(this.modalContent.nativeElement);
      // abp.ui.unblock(this.modalContent.nativeElement);

      // TODO: start watching for service messsages

      this._accountValidationService.validateAccount(input)
        .subscribe((output) => {
            console.log("_accountValidationService.validateAccount");
            console.log(output);
            // TODO: finish watching for service messsages
        });



      // this._emailSettingsService.getByUserId(this.appSession.userId)
      //     .finally(() => {  })
      //     .subscribe((emailSettings) => {
      //         this.emailSettings = emailSettings;
      //         this.active = true;
      //         this.modal.show();
      //     });

      this.modal.show();

    //   setInterval(
    //       ()=>{
    //           this.updateModal();
    //       },
    //       1000
    //   )
  }

  testMessages: string[] = [
    "Authenticating Acount",
    "* Connecting to server",
    "* Attempting Imap Authentication",
    "* ....",
    "* Opening Inbox",
    "* ....",
    "* Success",
    "* ....",
    "* Attempting Smtp Authentication",
    "* ....",
    "* Success",
    "* ....",
    "* Account Validated"
  ];

  messageNumber: number = 0;
  updateModal(): void {
    if(this.messageNumber < this.testMessages.length) {
        this.messages += this.testMessages[this.messageNumber] + "\r\n";
        this.messageNumber++;
        this.percent = Math.floor( (this.messageNumber / this.testMessages.length) * 100 ) + "%";
        this.scrollContentToBottom();
    }
  }

  onShown(): void {
      ($ as any).AdminBSB.input.activate($(this.modalContent.nativeElement));
  }

  l(name: string):string{
      return name;
  }

  save(): void {
      this.saving = true;

/*
      this._emailSettingsService.update(this.emailSettings)
          .finally(() => { 
            this.saving = false;
            abp.ui.unblock(this.modalContent.nativeElement);
           })
          .subscribe(() => {
              this.notify.info(this.l('SavedSuccessfully'));
              this.close();
              this.modalSave.emit(null);
          });
*/

    this.saving = false;  
  }

  testAuthenticationDetails(): void
  {
      alert("not implemented");
  }

  close(): void {
      this.active = false;
      this.modal.hide();
  }

}
