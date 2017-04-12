app = angular.module('myApp', ['ngAnimate', 'ngSanitize', 'ui.bootstrap', 'ngMaterial', 'ngMessages']);

(function(module) {
    module.controller('WeekController',
        function($uibModal, $http, $scope) {
            var ctrl = this;
            ctrl.Weeknumber = 1;
            ctrl.TimeTable = null;
            ctrl.User = null;
            ctrl.Today = new Date();
            ctrl.selectedTab = (ctrl.Today.getDay() - 1);
            ctrl.WeekMin = null;
            ctrl.WeekMax = null;
            ctrl.MinDate = getMonday(new Date());
            ctrl.Maxdate = new Date(+new Date + 12096e5);

            function getMonday(d) {
                d = new Date(d);
                var day = d.getDay(),
                    diff = d.getDate() - day + (day == 0 ? -6 : 1); // adjust when day is sunday
                return new Date(d.setDate(diff));
            }

            $scope.init = function(timetable) {
                ctrl.TimeTable = timetable;
                ctrl.Weeknumber = timetable.WeekNumber;
                ctrl.User = timetable.User;
                ctrl.WeekMin = "2017-W" + ctrl.Weeknumber.toString();
                ctrl.WeekMax = ctrl.Today.getFullYear().toString() + "-W" + (ctrl.Weeknumber + 1).toString();
            }

            ctrl.weekchange = function() {
                ctrl.getNewTimeTable(ctrl.Today);
            }

            ctrl.getNewTimeTable = function(day) {
                if (day) {
                    $http.post('/Home/GetTimeTableByDay',
                        {
                            day: day.toISOString()
                        })
                        .then(function(response, status, headers, config) {
                            if (response.data) {
                                ctrl.Weeknumber = response.data.WeekNumber;
                                ctrl.TimeTable = response.data;
                            }
                        });
                } else {
                    alert("Du kan kun book 14 dage frem i tiden");
                }

            }

            ctrl.getUserViewModel = function() {
                $http.post('Home/GetBookingViewModel')
                    .then(function(response, status, headers, config) {
                        if (response.data) {
                            ctrl.User = response.data;
                        }
                    });
            }

            ctrl.bookTime = function(washtime) {
                if (Date.parse(washtime.Time) > ctrl.Today.getTime()) {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'Bookng.html',
                        controller: 'BookingController',
                        controllerAs: 'ctrl',
                        resolve: {
                            currentTime: function() {
                                return washtime;
                            },

                            user: function() {
                                return ctrl.User;
                            }

                        }
                    });

                    modalInstance.result.then(function(result) {

                        },
                        function() {
                        });
                } else {
                    alert("Denne vasketid kan ikke længere bookes");
                };
            }
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

app.controller('BookingController', function ($uibModalInstance, $http, currentTime, user) {
    var ctrl = this;
    ctrl.currentTime = currentTime;
    ctrl.User = user;

    ctrl.ok = function () {
        ctrl.currentTime.RoomNumber = ctrl.User.RoomNumber;
        $http.post('Home/BookTime', ctrl.currentTime)
            .then(function (response, status, headers, config) {
                if (response.data) {
                    ctrl.success = true;
                    ctrl.Cancelsuccess  = false;
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
                    ctrl.success = false;
                    ctrl.Cancelsuccess = true;
                    ctrl.currentTime.IsBooked = false;
                    ctrl.RoomNumber = 0;
                }
            });
        }
    }
});
