//app module.ts

/* Angular Modules */
import { NgModule }         from '@angular/core';
import { BrowserModule }    from '@angular/platform-browser';
import { FormsModule }      from '@angular/forms';
import { HttpModule}        from '@angular/http';

import { Router }           from '@angular/router';

// Application Components 
import { AppComponent }                         from './app.component';
import { AllJobsComponent }                     from './home/all-jobs.component';
import { JobDetailComponent }                   from './home/job-detail.component';
import { AppRoutingModule, routedComponents }   from './app-routing.module';
import { NavbarComponent }                      from './navbar/navbar.component';
import { AllJobsService }                       from './home/all-jobs.service';

@NgModule({
    imports: [
        BrowserModule,
        FormsModule,
        HttpModule,
        AppRoutingModule
    ],
    declarations: [
        AppComponent,
        AllJobsComponent,
        JobDetailComponent,
        routedComponents,
        NavbarComponent
        /* more components will be added later  */
    ],
    providers: [AllJobsService],
    bootstrap: [AppComponent]
})

export class AppModule { }

