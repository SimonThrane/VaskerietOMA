app = angular.module('myApp', ['ngAnimate', 'ngSanitize', 'ui.bootstrap', 'ngMaterial', 'ngMessages']);

(function(module) {
    module.controller('WeekController',
        function($uibModal, $http, $scope) {
            var ctrl = this;
            ctrl.Weeknumber = 1;
            ctrl.TimeTable = null;

            $scope.init = function(timetable) {
                ctrl.TimeTable = timetable;
                ctrl.Weeknumber = timetable.WeekNumber;

            }

            ctrl.bookTime = function(washtime) {
                var modalInstance = $uibModal.open({
                    animation: true,
                    templateUrl: 'Bookng.html',
                    controller: 'BookingController',
                    controllerAs: 'ctrl',
                    resolve: {
                        currentTime: function() {
                            return washtime;
                        }
                    }
                });

                modalInstance.result.then(function(result) {

                    },
                    function() {
                    });
            };


        });
})(angular.module('myApp'));


app.directive('roomValidator', function () {
    return {
        require: 'ngModel',
        link: function (scope, element, attr, mCtrl) {
            function myValidation(value) {
                if (value > 101 && value < 1) {
                    mCtrl.$setValidity('roomValid', true);
                } else {
                    mCtrl.$setValidity('roomValid', false);
                }
                return value;
            }
            mCtrl.$parsers.push(myValidation);
        }
    };
});

app.controller('BookingController', function ($uibModalInstance, $http, currentTime) {
    var ctrl = this;
    ctrl.currentTime = currentTime;


    ctrl.ok = function () {
        $http.post('Home/BookTime', ctrl.currentTime)
            .then(function (response, status, headers, config) {
                if (response.data) {
                    ctrl.success = true;
                    ctrl.currentTime.IsBooked = true;
                }
            });

    };

    ctrl.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

    ctrl.CancelBooking = function () {
        if (confirm("Are you sure?")) {
            $http.post('Home/CancelBooking', ctrl.currentTime)
            .then(function (response, status, headers, config) {
                if (response.data) {
                    ctrl.Cancelsuccess = true;
                    ctrl.currentTime.IsBooked = false;
                    ctrl.RoomNumber = 0;
                }
            });
        }
    }
});
