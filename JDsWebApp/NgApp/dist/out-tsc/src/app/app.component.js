"use strict";
/* Application Component */
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var AppComponent = (function () {
    function AppComponent() {
        this.title = "JD's Angular Application";
    }
    return AppComponent;
}());
AppComponent = __decorate([
    core_1.Component({
        moduleId: module.id,
        selector: 'app',
        template: "\n        <nav>\n            <a routerLink=\"/home\" routerLinkActive=\"active\">Home Page</a>\n            <a routerLink=\"/about\" routerLinkActive=\"active\">About</a>\n            <a routerLink=\"/contactform\" routerLinkActive=\"active\">Contact Us</a>\n        </nav>\n        <h1>{{title}}</h1>\n        <a routerLink=\"/jobs\" routerLinkActive=\"active\">Jobs</a>\n        <router-outlet></router-outlet>\n    ",
        styleUrls: ['./app.component.css']
    })
], AppComponent);
exports.AppComponent = AppComponent;
//# sourceMappingURL=app.component.js.map