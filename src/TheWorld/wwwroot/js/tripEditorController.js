(function () {
    "use strict";
    angular.module("app-trips")
    .controller("tripEditorController", tripEditorController);

    function tripEditorController($routeParams,$http) {
        var vm = this;
        //vm.name = "Surya";
        vm.tripName = $routeParams.tripName;
        vm.stops = [];
        vm.errorMessage = "";
        vm.isBusy = true;
        vm.newStop = {};
        //vm.arrival=";"
        //vm.name=";"
        var url = "/api/trips/" + vm.tripName + "/stops";
        $http.get(url)
        .then(function (response) {
            //success
            angular.copy(response.data, vm.stops);           
                _showMap(vm.stops); //after getting data from server.
            
            },
            function (error) {
                //failure
                vm.errorMessage = "failed to load stops" + error;
            })
            .finally(function () {
            vm.isBusy = false;
        });

        vm.addStop = function () { //adding stops info
            vm.isBusy = true;
            //vm.newStop={arrival:vm.arrival,name:vm.name}
            $http.post(url, vm.newStop)
            .then(function (response) {
                //success
                vm.stops.push(response.data);
                _showMap(vm.stops);
                vm.newStop = {};
            }, function (err) {
                //failure
                vm.errorMessage = "Failed to add new stop";
            })
            .finally(function () {
                vm.isBusy = false;
            });
        };
    }

    //not exposed to global scope
    function _showMap(stops) {   //_ for to represent as prvate function.
        if (stops && stops.length > 0) {
            //this is to supply data in the format that API wants it.
            var mapStops = _.map(stops, function (item) {
                return {
                    lat: item.latitude,
                    long: item.longitude,
                    info : item.name
                };
            });    // this underscore lib maps one type to other on client side
            //show Map
            travelMap.createMap({
                stops: mapStops,
                selector: "#map", //used css selector
                currentStop: 1,
                initialZoom: 3
            });
        };

    }

})();