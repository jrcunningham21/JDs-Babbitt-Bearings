"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var protractor_1 = require("protractor");
var StarterAppPage = (function () {
    function StarterAppPage() {
    }
    StarterAppPage.prototype.navigateTo = function () {
        return protractor_1.browser.get('/');
    };
    StarterAppPage.prototype.getParagraphText = function () {
        return protractor_1.element(protractor_1.by.css('app-root h1')).getText();
    };
    return StarterAppPage;
}());
exports.StarterAppPage = StarterAppPage;
//# sourceMappingURL=app.po.js.map