/* Application Component */

import { Component }        from '@angular/core';

@Component({
    moduleId: module.id,
    selector: 'app',
    template: 
    `
        <nav>
            <a routerLink="/home" routerLinkActive="active">Home Page</a>
            <a routerLink="/about" routerLinkActive="active">About</a>
            <a routerLink="/contactform" routerLinkActive="active">Contact Us</a>
        </nav>
        <h1>{{title}}</h1>
        <a routerLink="/jobs" routerLinkActive="active">Jobs</a>
        <router-outlet></router-outlet>
    `,
    styleUrls: ['./app.component.css']
})

export class AppComponent{ title = "JD's Angular Application"}