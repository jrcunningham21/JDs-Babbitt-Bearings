/* App Routing Module */

import { NgModule }             from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { AllJobsComponent }     from './home/all-jobs.component';

const routes: Routes = [
    {path: '', redirectTo: '/Home', pathMatch: 'full'},
    {path: 'Home', component: AllJobsComponent }
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export class AppRoutingModule { }

export const routedComponents = []