import { Injectable }    from '@angular/core';
import { Http, Headers } from '@angular/http';

import 'rxjs/add/operator/toPromise';

import { Job } from './job';

@Injectable()
export class AllJobsService {
    private headers = new Headers({'Content-Type': 'application/json'});
    private jobsUrl = '/api/Home/id?'; // there currently is not an api built out in the current jd's app for angular to function with 
                                        //this route is not working because of that.

    constructor(private http: Http) {}

    getAllJobs(): Promise<Job[]> {
        return this.http.get(this.jobsUrl)
        .toPromise()
        .then(response => response.json().data as Job[])
        .catch(this.handleError);
    }

    private handleError(error: any): Promise<any> {
        console.error('Error Occured', error);
        return Promise.reject(error.message || error);
    }
}