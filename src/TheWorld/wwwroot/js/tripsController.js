(function () {
    "use strict";

    //getting the existing module
    angular.module("app-trips")
        .controller("tripsController", tripsController);

    function tripsController($scope, $http) {

        var vm = this;
        //vm.name = "Surya";
        //vm.trips = [{
        //    name: "Us Trip",
        //    created: new Date()
        //},
        //{
        //    name: "World Trip",
        //    created: new Date()
        //}];
        //getting it from server using api call 

        vm.trips = [];
        vm.newTrip = {};
        vm.errorMessage = "";
        vm.isBusy = true;
        $http.get("/api/trips")
            .then(function (response) {
                //success
                angular.copy(response.data, vm.trips)
            },
            function (error) {
                vm.errorMessage = "failed to load data " + error;
            })
            .finally(function () {
                vm.isBusy = false; //this flag decides the spinner to show or not
            });
        //commented to implement actual functionality
        vm.addTrip = function () {
            //    vm.trips.push({ name: vm.newTrip.name, created: new Date() });
            //    vm.newTrip = {};
            vm.isBusy = true;
            vm.errorMessage = "";
            $http.post("/api/trips", vm.newTrip)
                .then(function (response) {
                    //success
                    vm.trips.push(response.data);
                    vm.newTrip = {};
                }, function () {
                    //failure
                    vm.errorMessage = "Failed to save new trip";
                })
                .finally(function () {
                    vm.isBusy = false;
                });
        };

    };
})();