app = angular.module('myApp', ['ngAnimate', 'ngSanitize', 'ui.bootstrap']);
angular.module('myApp').controller('BarBookingController', function ($scope, $uibModal, $filter) {
    var ctrl = this;
    ctrl.HasBookings = false;
    ctrl.Bookings = [];
    ctrl.TodaysEvent = [];
    ctrl.today = function () {
        ctrl.dt = new Date();
    };
    ctrl.today();

    ctrl.Available = true;
    $scope.init = function (barBookings) {
        ctrl.Bookings = barBookings.BarBookings;
        ctrl.TodaysEvent = $.grep(ctrl.Bookings, function (e) {
            return e.StartTime.substr(0,10) === ctrl.dt.toISOString().substr(0,10);
        });
        hasBooking();
    }

    ctrl.options = {
        customClass: getDayClass,
        showWeeks: true
    };

    ctrl.gotoDay = function(day) {
        ctrl.TodaysEvent = $.grep(ctrl.Bookings, function (e) {
            return e.StartTime.substr(0, 10) === day.toISOString().substr(0, 10);
        });
        hasBooking();
        ctrl.Available = day >= new Date();
    }

    function hasBooking() {
        if (ctrl.TodaysEvent.length > 0) {
            ctrl.HasBookings = true;
        } else {
            ctrl.HasBookings = false;
        }
    }

    ctrl.setDate = function (year, month, day) {
        ctrl.dt = new Date(year, month, day);
    };

    ctrl.bookBar = function(day) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'Bookng.html',
            controller: 'BookingController',
            controllerAs: 'ctrl',
            resolve: {
                day: function() {
                    return day;
                }
            }
        });

        modalInstance.result.then(function(result) {
            ctrl.Bookings.push(result);
            ctrl.TodaysEvent = $.grep(ctrl.Bookings, function (e) {
                return e.StartTime.substr(0, 10) === day.toISOString().substr(0, 10);
            });

            hasBooking();

            ctrl.options = {
                customClass: getDayClass,
                showWeeks: true
            };
            },
            function() {
            });
    }

    function getDayClass(data) {
        var date = data.date,
        mode = data.mode;
        if (mode === 'day') {
        for (var i = 0; i < ctrl.Bookings.length; i++) {
            var currentDay = ctrl.Bookings[i].StartTime.substr(0, 10);
            if (mode === 'day') {
                if (date.toISOString().substr(0,10) === currentDay) 
                {
                        return "partially";
                    }
                }
            }
            return "";
        }
   }
});

    app.controller('BookingController', function ($uibModalInstance, $http, day) {
        var ctrl = this;
        ctrl.CurrentBooking = new Object();
        ctrl.CurrentBooking.StartTime = day;
        ctrl.CurrentBooking.EndTime = day;
        ctrl.CurrentBooking.IsPublic = true;

        ctrl.ok = function () {
            $http.post('/Home/BookBar', ctrl.CurrentBooking)
                .then(function (response, status, headers, config) {
                    if (response.data) {
                        $uibModalInstance.close(response.data);
                    }
                });

    };

    ctrl.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

});
