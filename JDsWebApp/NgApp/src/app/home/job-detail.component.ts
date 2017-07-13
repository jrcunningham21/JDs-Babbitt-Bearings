import { Component } from '@angular/core';
import { Job }       from './job';

@Component({
    moduleId: module.id,
    selector: 'job-detail',
    template: `
        <div *ngIf="job">
            <h4> {{job.JobID}} details</h4>
            <div><label>Job ID: </label>{{job.JobID}}</div>
        </div>
    `
})

export class JobDetailComponent {
    job: Job;
}