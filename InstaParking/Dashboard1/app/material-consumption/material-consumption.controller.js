(function () {
    'use strict';

    angular.module('app')
        .controller('MaterialconsumptionCtrl', ['$scope', MaterialconsumptionCtrl])

    function MaterialconsumptionCtrl($scope) {
        
        
    // table code

    $scope.selected = [];
        $scope.limitOptions = [5, 10, 15];
        
        $scope.options = {
            rowSelection: true,
            multiSelect: true,
            autoSelect: true,
            decapitate: false,
            largeEditDialog: false,
            boundaryLinks: false,
            limitSelect: true,
            pageSelect: true
        };
        
        $scope.query = {
            order: 'name',
            limit: 5,
            page: 1
        };
        
        $scope.goldconsumption = {
            "count": 9,
            "data": [
                {
                    "date": "24-01-2020",
                    "assayername": "Raju",
                    "openingbalance": "6",
                    "usedweight": "1.5",
                    "closingbalance": "4.5",
                    "approved": "Manager",
                    "comments": "Comment"
                }, {
                    "date": "23-01-2020",
                    "assayername": "Venkat",
                    "openingbalance": "8.5",
                    "usedweight": "2.5",
                    "closingbalance": "6",
                    "approved": "Manager",
                    "comments": "Comment"
                }, {
                    "date": "22-01-2020",
                    "assayername": "Srinu",
                    "openingbalance": "9",
                    "usedweight": "0.5",
                    "closingbalance": "8.5",
                    "approved": "Manager",
                    "comments": "Comment"
                }
            ]
        };
        
        $scope.editComment = function (event, dessert) {
            event.stopPropagation(); // in case autoselect is enabled            
            
            var promise;
            
            if($scope.options.largeEditDialog) {
                promise = $mdEditDialog.large(editDialog);
            } else {
                promise = $mdEditDialog.small(editDialog);
            }
            
            promise.then(function (ctrl) {
                var input = ctrl.getInput();
                
                input.$viewChangeListeners.push(function () {
                    input.$setValidity('test', input.$modelValue !== 'test');
                });
            });
        };
        
        $scope.toggleLimitOptions = function () {
            $scope.limitOptions = $scope.limitOptions ? undefined : [5, 10, 15];
        };
        
        $scope.getTypes = function () {
            return ['Candy', 'Ice cream', 'Other', 'Pastry'];
        };
        
        $scope.loadStuff = function () {
            $scope.promise = $timeout(function () {
                // loading
            }, 2000);
        }
        
        $scope.logItem = function (item) {
            console.log(item.name, 'was selected');
        };
        
        $scope.logOrder = function (order) {
            console.log('order: ', order);
        };
        
        $scope.logPagination = function (page, limit) {
            console.log('page: ', page);
            console.log('limit: ', limit);
        }

        //select fields        
        $scope.customerddl = {};
        $scope.customerddl.customername = '';
        $scope.customerddl.customer = [
        {'abbrev':'RS Brothers'},
        {'abbrev':'Chennai Shopping Mall'},
        {'abbrev':'Lalitha Jewellery'}]

        $scope.customerpocddl = {};
        $scope.customerpocddl.customerpocname = '';
        $scope.customerpocddl.customerpoc = [
        {'abbrev':'Umakanth'},
        {'abbrev':'Sagar'},
        {'abbrev':'Ramarao'}] 
        
        $scope.carrierddl = {};
        $scope.carrierddl.carriercname = '';
        $scope.carrierddl.carrier = [
        {'abbrev':'Krishna'},
        {'abbrev':'Venkat'},
        {'abbrev':'Suresh'}]

        }

         
     
})(); 
