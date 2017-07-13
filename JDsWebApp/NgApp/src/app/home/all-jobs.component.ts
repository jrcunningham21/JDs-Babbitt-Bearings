import { Component, OnInit }    from '@angular/core';
import { Router }               from '@angular/router';

import { AllJobsService }       from './all-jobs.service';
import { Job }                  from './job';


@Component({
    moduleId: module.id,
    selector: 'all-jobs',
    templateUrl: './all-jobs.component.html'
})

export class AllJobsComponent implements OnInit {
    jobs: Job[];
    selectedJob: Job;



    //constructor
    constructor(private jobService: AllJobsService, 
                private router: Router) {}

    getJobs() : void {
        this.jobService.getAllJobs()
        .then(jobs => this.jobs = jobs);
    }

    ngOnInit() { this.jobService.getAllJobs(); }
 }