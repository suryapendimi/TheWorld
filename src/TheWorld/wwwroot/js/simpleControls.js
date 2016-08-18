//simpleControls.js (directive)
//to use it in multiple places
(function () {
    "use strict";
    angular.module("simpleControls", [])
        .directive("waitCursor", waitcursor);

    function waitcursor() {
        return {
            scope:{
                show: "=displayWhen" //attrivbute //can have more than one attribute
                //displayWhen is a compound word it needs to be configured as display-when in html
            },
            restrict: "E", //restrict only to element style like <wait-cursor>
            templateUrl: "/views/waitCursor.html"
        };
    };
})();