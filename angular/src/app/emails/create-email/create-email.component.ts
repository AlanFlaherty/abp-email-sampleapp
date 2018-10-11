import { Component, OnInit } from '@angular/core';
import { validateConfig } from '@angular/router/src/config';
import { appModuleAnimation } from '@shared/animations/routerTransition';

@Component({
  selector: 'app-create-email',
  templateUrl: './create-email.component.html',
  styleUrls: ['./create-email.component.css'],
  animations: [appModuleAnimation()]
})
export class CreateEmailComponent implements OnInit {

  to: string;
  subject: string;
  content: string;

  constructor() { }

  ngOnInit() {
    this.content = "";
  }

  getPlainTextContent(): string {
    // TODO: replace this with a proper library! strip_tags.
    var tmp = document.createElement("DIV");
    tmp.innerHTML = this.content;
    tmp.remove();
    let plainText:string = tmp.textContent || tmp.innerText;

    // release the node, TODO: check if this is enough or more needs to happen like https://toranbillups.com/blog/archive/2009/04/21/Cleanup-for-dynamically-generated-DOM-elements-in-IE/
    tmp = null;

    return plainText;
  }

  sendEmail() : void {
    // this.emailService.sendEmail(this.subject, this.to, this.content, this.getPlainTextContent());
  }
}
