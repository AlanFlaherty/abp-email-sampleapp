import { Component, OnInit } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';

@Component({
  selector: 'app-email-dashboard',
  templateUrl: './email-dashboard.component.html',
  styleUrls: ['./email-dashboard.component.css'],
  animations: [appModuleAnimation()]
})
export class EmailDashboardComponent implements OnInit {

  accountsAvailable : boolean = false;
  constructor() { }

  ngOnInit() {
    
  }
}
