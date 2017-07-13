"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var router_1 = require("@angular/router");
var all_jobs_service_1 = require("./all-jobs.service");
var AllJobsComponent = (function () {
    //constructor
    function AllJobsComponent(jobService, router) {
        this.jobService = jobService;
        this.router = router;
    }
    AllJobsComponent.prototype.getJobs = function () {
        var _this = this;
        this.jobService.getAllJobs()
            .then(function (jobs) { return _this.jobs = jobs; });
    };
    AllJobsComponent.prototype.ngOnInit = function () { this.jobService.getAllJobs(); };
    return AllJobsComponent;
}());
AllJobsComponent = __decorate([
    core_1.Component({
        moduleId: module.id,
        selector: 'all-jobs',
        templateUrl: './all-jobs.component.html'
    }),
    __metadata("design:paramtypes", [all_jobs_service_1.AllJobsService,
        router_1.Router])
], AllJobsComponent);
exports.AllJobsComponent = AllJobsComponent;
//# sourceMappingURL=all-jobs.component.js.map