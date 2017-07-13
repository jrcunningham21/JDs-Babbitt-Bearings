"use strict";
//app module.ts
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
/* Angular Modules */
var core_1 = require("@angular/core");
var platform_browser_1 = require("@angular/platform-browser");
var forms_1 = require("@angular/forms");
var http_1 = require("@angular/http");
// Application Components 
var app_component_1 = require("./app.component");
var all_jobs_component_1 = require("./home/all-jobs.component");
var job_detail_component_1 = require("./home/job-detail.component");
var app_routing_module_1 = require("./app-routing.module");
var navbar_component_1 = require("./navbar/navbar.component");
var all_jobs_service_1 = require("./home/all-jobs.service");
var AppModule = (function () {
    function AppModule() {
    }
    return AppModule;
}());
AppModule = __decorate([
    core_1.NgModule({
        imports: [
            platform_browser_1.BrowserModule,
            forms_1.FormsModule,
            http_1.HttpModule,
            app_routing_module_1.AppRoutingModule
        ],
        declarations: [
            app_component_1.AppComponent,
            all_jobs_component_1.AllJobsComponent,
            job_detail_component_1.JobDetailComponent,
            app_routing_module_1.routedComponents,
            navbar_component_1.NavbarComponent
            /* more components will be added later  */
        ],
        providers: [all_jobs_service_1.AllJobsService],
        bootstrap: [app_component_1.AppComponent]
    })
], AppModule);
exports.AppModule = AppModule;
//# sourceMappingURL=app.module.js.map