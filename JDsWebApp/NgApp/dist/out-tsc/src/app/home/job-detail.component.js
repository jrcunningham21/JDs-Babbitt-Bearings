"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var JobDetailComponent = (function () {
    function JobDetailComponent() {
    }
    return JobDetailComponent;
}());
JobDetailComponent = __decorate([
    core_1.Component({
        moduleId: module.id,
        selector: 'job-detail',
        template: "\n        <div *ngIf=\"job\">\n            <h4> {{job.JobID}} details</h4>\n            <div><label>Job ID: </label>{{job.JobID}}</div>\n        </div>\n    "
    })
], JobDetailComponent);
exports.JobDetailComponent = JobDetailComponent;
//# sourceMappingURL=job-detail.component.js.map