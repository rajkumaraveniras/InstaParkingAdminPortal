(function () {
    'use strict';

    angular.module('app')
        .controller('EmployeeCtrl', ['$scope', '$state', '$stateParams', '$mdDialog', EmployeeCtrl]);

    function EmployeeCtrl($scope, $state, $stateParams, $mdDialog) {

        ModelDefinations();
        function ModelDefinations() {
            $scope.RolesListModel = [];
            $scope.RoleModel = {
                'UserTypeID': '',
                'UserTypeCode': '',
                'UserTypeName': '',
                'UserTypeDesc': '',
                'IsActive': '',
                'IsDeleted': '',
                'UpdatedBy': ''
            };


            $scope.ActiveRolesListModel = [];
            $scope.EmployeesListModel = [];
            $scope.EmployeesModel = {
                'UserID': '',
                'UserTypeID': '',
                'UserCode': '',
                'UserName': '',
                'SupervisorID': '',
                'IsActive': '',
                'UpdatedBy': '',
                'Password': '',
                'PhoneNumber': '',
                'AltPhoneNumber': '',
                'JoiningDate': '',
                'Salary': '',
                'EPFAccountNumber': '',
                'Address': '',
                // 'AssignedLocationID': '',
                //  'AssignedLotID': '',
                'AadharNumber': '',
                'PANNumber': ''
                , 'IsOperator': ''
                , 'UserTypeName': ''
              
            };
            $scope.ModulesList = [];

            $scope.SupervisorListModel = [];


            $scope.UserLocationMapperListModel = [];
            $scope.UserLocationMapperModel = {
                'UserLocationMapperID': '',
                'UserID': '',
                'UserName': '',
                'LocationID': '',
                'LocationName': '',
                'LocationParkingLotID': '',
                'LocationParkingLotName': '',
                'IsActive': '',
                'IsDeleted': '',
                'UpdatedBy': '',
                'UpdatedOn': ''
            };



            $scope.AssignListModel = [];
            $scope.AssignModel = {
                'UserLocationMapperID': '',
                'UserID': '',
                'LocationID': '',
                'IsActive': '',
                'IsDeleted': '',
                'UpdatedBy': '',
                'UserCode': '',
                'UserName': '',
                'LocationName': '',
                'LotID': '',
                'LotName': ''
            };

            $scope.GetActiveEmployeeListModel = [];
            $scope.GetActiveLocationListModel = [];
            $scope.GetActiveLotListModel = [];


            $scope.required = true;
        }
        //Roles Code Start
        GetRolesList();
        EditRoleByID();
        function GetRolesList() {
            var url = $("#GetRolesList").val();
            if (url != undefined) {
                $('#loader-container').show();
                if (CheckInSession()) {
                    $.ajax({
                        type: "POST",
                        url: url,
                        data: "",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            if (data.length > 0) {
                                $scope.RolesListModel = data;
                                $scope.$apply();
                                for (var items = 0; items < $scope.RolesListModel.length; items++) {
                                    if ($scope.RolesListModel[items].IsActive == true) {
                                        $scope.RolesListModel[items].IsActive = 'Active';
                                    }
                                    else {
                                        $scope.RolesListModel[items].IsActive = 'Inactive';
                                    }
                                }
                                $scope.sortpropertyName = 'UserTypeName';
                                $scope.sortorder = false;
                                $scope.$apply();
                            }
                            $('#loader-container').hide();
                        }, error: function (data) {
                            $('#loader-container').hide();
                        }
                    });
                }
                else {
                    window.location.href = $("#LogOut").val();
                }
            }
        }
        $scope.SubmitRole = function () {
            var url = $("#SaveRole").val();
            var RoleID;
            if ($scope.RoleModel.UserTypeID == "") {
                RoleID = 0;
            }
            else {
                RoleID = $scope.RoleModel.UserTypeID;
            }
            $scope.RoleModel.UserTypeID = RoleID;
            $('#loader-container').show();
            if (CheckInSession()) {
                $.ajax({
                    type: "POST",
                    url: url,
                    data: "{'RolesData':" + JSON.stringify($scope.RoleModel) + "}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {
                        if (data != "Failed") {
                            alert("Role Created Successfully");
                            $state.go("employee/roles");
                        }
                        $('#loader-container').hide();
                    },
                    error: function (data) {
                        $('#loader-container').hide();
                    }
                });
            }
            else {
                window.location.href = $("#LogOut").val();
            }
        };
        function EditRoleByID() {
            var url = $("#ViewRole").val();
            var hdnFlagVal = $stateParams.roleid;

            if (hdnFlagVal != undefined && url != undefined) {
                $('#loader-container').show();
                if (CheckInSession()) {

                    $.ajax({
                        type: "POST",
                        url: url,
                        data: "{'RoleID':" + hdnFlagVal + "}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            if (data.length > 0) {
                                for (var i = 0; i < data.length; i++) {
                                    $scope.RoleModel.UserTypeID = data[i]["UserTypeID"];
                                    $scope.RoleModel.UserTypeCode = data[i]["UserTypeCode"];
                                    $scope.RoleModel.UserTypeName = data[i]["UserTypeName"];
                                    $scope.RoleModel.UserTypeDesc = data[i]["UserTypeDesc"];
                                    $scope.RoleModel.IsActive = data[i]["IsActive"];
                                    $scope.$apply();
                                }
                            }
                            $('#loader-container').hide();
                        },
                        error: function (data) {
                            $('#loader-container').hide();
                        }
                    });
                }
                else {
                    window.location.href = $("#LogOut").val();
                }
            }
        }
        $scope.UpdateRole = function () {
            var url = $("#UpdateRole").val();
            $scope.RoleModel.UserTypeID = $stateParams.roleid;

            if (url != undefined) {
                $('#loader-container').show();
                if (CheckInSession()) {

                    $.ajax({
                        type: "POST",
                        url: url,
                        data: "{'RolesData':" + JSON.stringify($scope.RoleModel) + "}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            if (data != "Failed") {
                                alert("Role Updated Successfully");
                                $state.go("employee/roles");
                            }
                            $('#loader-container').hide();
                        },
                        error: function (data) {
                            $('#loader-container').hide();
                        }
                    });
                }
                else {
                    window.location.href = $("#LogOut").val();
                }
            }
        };
        //Roles Code End

        //Employees Code Start
        GetEmployeesList();
        GetActiveRolesList();
        GetActiveSupervisorsList();
        EditEmployeeByID();

        function GetEmployeesList() {
            var url = $("#GetEmployeesList").val();
            if (url != undefined) {
                $('#loader-container').show();
                if (CheckInSession()) {
                    $.ajax({
                        type: "POST",
                        url: url,
                        data: "",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            if (data.length > 0) {
                                $scope.EmployeesListModel = data;
                                $scope.$apply();//16122020
                                for (var items = 0; items < $scope.EmployeesListModel.length; items++) {
                                    if ($scope.EmployeesListModel[items].IsActive == true) {
                                        $scope.EmployeesListModel[items].IsActive = 'Active';
                                    }
                                    else {
                                        $scope.EmployeesListModel[items].IsActive = 'Inactive';
                                    }
                                }
                                $scope.sortpropertyName = 'UserName';
                                $scope.sortorder = false;
                                $scope.$apply();//16122020
                            }
                            $('#loader-container').hide();
                        }, error: function (data) {
                            $('#loader-container').hide();
                        }
                    });
                }
                else {
                    window.location.href = $("#LogOut").val();
                }
            }
        }

        function GetActiveRolesList() {
            var url = $("#GetActiveRolesList").val();
            if (url != undefined) {
                $('#loader-container').show();
                if (CheckInSession()) {
                    $.ajax({
                        type: "POST",
                        url: url,
                        data: "",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            if (data.length > 0) {
                                $scope.ActiveRolesListModel = data;
                                //for (var items = 0; items < $scope.RolesListModel.length; items++) {
                                //    if ($scope.RolesListModel[items].IsActive == true) {
                                //        $scope.RolesListModel[items].IsActive = 'Active';
                                //    }
                                //    else {
                                //        $scope.RolesListModel[items].IsActive = 'Inactive';
                                //    }
                                //}
                                $scope.$apply();//16122020
                            }
                            $('#loader-container').hide();
                        }, error: function (data) {
                            $('#loader-container').hide();
                        }
                    });
                }
                else {
                    window.location.href = $("#LogOut").val();
                }
            }
        }
        function GetActiveSupervisorsList() {
            var url = $("#GetActiveSupervisorsList").val();
            if (url != undefined) {
                $('#loader-container').show();
                if (CheckInSession()) {
                    $.ajax({
                        type: "POST",
                        url: url,
                        data: "",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            if (data.length > 0) {
                                $scope.SupervisorListModel = data;
                                $scope.$apply();//16122020
                            }
                            $('#loader-container').hide();
                        }, error: function (data) {
                            $('#loader-container').hide();
                        }
                    });
                }
                else {
                    window.location.href = $("#LogOut").val();
                }
            }
        }

        $scope.SubmitEmployee = function () {

            //var locationids = $scope.EmployeesModel.AssignedLocationID;
            //var lotid = $scope.EmployeesModel.AssignedLotID;

            //if (lotid == "") {
            //    var locids = "";
            //    for (var x = 0; x < locationids.length; x++)
            //    {
            //        locids += locationids[x] + ",";
            //    }
            //    locids = locids.substring(0, locids.length - 1);
            //    $scope.EmployeesModel.AssignedLocationID = locids;
            //}


            var photosize = 0;
            var aadharsize = 0;
            var PANsize = 0;

            var fdata = new FormData();
            var photoLogo = $("#Pictureupload").get(0);
            var files1 = photoLogo.files;
            if (files1.length > 0) {
                for (var i = 0; i < files1.length; i++) {
                    fdata.append("EmpPhotoLogo", files1[i]);
                }
                photosize = $('#Pictureupload')[0].files[0].size;
            }


            var AadharLogo = $("#Aadharupload").get(0);
            var files2 = AadharLogo.files;
            if (files2.length > 0) {
                for (var j = 0; j < files2.length; j++) {
                    fdata.append("AadharProof", files2[j]);
                }
                aadharsize = $('#Aadharupload')[0].files[0].size;
            }


            var PANLogo = $("#Panupload").get(0);
            var files3 = PANLogo.files;
            if (files3.length > 0) {
                for (var k = 0; k < files3.length; k++) {
                    fdata.append("PANProof", files3[k]);
                }
                PANsize = $('#Panupload')[0].files[0].size;
            }


            var url = $("#SaveEmployee").val();
            var UserID;
            if ($scope.EmployeesModel.UserID == "") {
                UserID = 0;
            }
            else {
                UserID = $scope.EmployeesModel.UserID;
            }
            $scope.EmployeesModel.UserID = UserID;
            $('#loader-container').show();
            if (CheckInSession()) {
                // if (files1.length > 0 && photosize <= 20480) {
                //if (files2.length > 0 && aadharsize <= 20480) {
                // if (files3.length > 0 && PANsize <= 20480) {
                $.ajax({
                    type: "POST",
                    url: url,
                    data: "{'UsersData':" + JSON.stringify($scope.EmployeesModel) + "}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {
                        if (data != "Failed" && data != "Data Exists") {

                            var sucessdata = data;
                            var profidenity;
                            if (sucessdata.length > 0) {
                                profidenity = sucessdata.split('@')[1];
                            }

                            $scope.EmployeesModel.UserID = profidenity;

                            //var url_UserStation = $("#AssignStationtoUser").val();
                            //$.ajax({
                            //    type: "POST",
                            //    url: url_UserStation,
                            //    data: "{'employeedata':" + JSON.stringify($scope.EmployeesModel) + "}",
                            //    contentType: "application/json; charset=utf-8",
                            //    dataType: "json",
                            //    success: function (data) {
                            //    },
                            //    error: function (data) {
                            //    }
                            //});



                            fdata.append('EmployeeDetails', JSON.stringify($scope.EmployeesModel));

                            var options = {};
                            options.url = "Handlers/EmployeeFileHandler.ashx";
                            options.type = "POST";
                            options.data = fdata;
                            options.contentType = false;
                            options.processData = false;
                            options.success = function (result) {
                            };
                            options.error = function (err) {
                            };

                            $.ajax(options);



                            alert("Employee Created Successfully");
                            GetSavedUserDetailsByID($scope.EmployeesModel.UserID);
                            $("#employeeBtn").attr("disabled", true);
                            $('input[type="file"]').attr("disabled", true);
                            //$state.go("employee/employees");
                        }
                        $('#loader-container').hide();
                    },
                    error: function (data) {
                        $('#loader-container').hide();
                    }
                });
                //}
                //else {
                //    alert("Upload PAN and Image Size should be less than or equal to 20KB.");
                //    $('#loader-container').hide();
                //}
                //}
                //else {
                //    alert("Upload Aadhar and Image Size should be less than or equal to 20KB.");
                //    $('#loader-container').hide();
                //}
                //} else {
                //    alert("Upload Employee Photo and Image Size should be less than or equal to 20KB.");
                //    $('#loader-container').hide();
                //}

            }
            else {
                window.location.href = $("#LogOut").val();
            }
        };
        function UrlExists(url) {
            var http = new XMLHttpRequest();
            http.open('HEAD', url, false);
            http.send();
            return http.status != 404;
        }
        $scope.ChangeUserType = function (usertypeID) {
            for (var i = 0; i < $scope.ActiveRolesListModel.length; i++) {
                if (usertypeID == $scope.ActiveRolesListModel[i].UserTypeID) {
                    if ($scope.ActiveRolesListModel[i].UserTypeName == 'Super Administrator'
                        || $scope.ActiveRolesListModel[i].UserTypeName == 'Administrator') {
                        $scope.required = false;
                    }
                    else {
                        $scope.required = true;
                    }
                }
            }
        };
        function GetSavedUserDetailsByID(userid) {
            var url = $("#GetSavedUserDetails").val();
            var hdnFlagVal = userid;

            if (hdnFlagVal != undefined && url != undefined) {
                $('#loader-container').show();
                if (CheckInSession()) {
                    $.ajax({
                        type: "POST",
                        url: url,
                        data: "{'EmployeeID':" + hdnFlagVal + "}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            if (data.length > 0) {
                                for (var i = 0; i < data.length; i++) {
                                    $scope.EmployeesModel.UserID = data[i]["UserID"];
                                    $scope.EmployeesModel.UserTypeID = data[i]["UserTypeID"];
                                    $scope.EmployeesModel.UserCode = data[i]["UserCode"];
                                    $scope.EmployeesModel.UserName = data[i]["UserName"];
                                    $scope.EmployeesModel.Password = data[i]["Password"];
                                    $scope.EmployeesModel.PhoneNumber = data[i]["PhoneNumber"];
                                    $scope.EmployeesModel.SupervisorID = data[i]["SupervisorID"];
                                    $scope.EmployeesModel.IsActive = data[i]["IsActive"];

                                    $scope.EmployeesModel.JoiningDate = new Date(data[i]["JoiningDate"]);
                                    $scope.EmployeesModel.Salary = data[i]["Salary"];
                                    $scope.EmployeesModel.EPFAccountNumber = data[i]["EPFAccountNumber"];
                                    $scope.EmployeesModel.AltPhoneNumber = data[i]["AltPhoneNumber"];
                                    $scope.EmployeesModel.Address = data[i]["Address"];
                                    $scope.EmployeesModel.AadharNumber = data[i]["AadharNumber"];
                                    $scope.EmployeesModel.PANNumber = data[i]["PANNumber"];

                                    $scope.EmployeesModel.IsOperator = data[i]["IsOperator"];

                                    $scope.ChangeUserType($scope.EmployeesModel.UserTypeID);//New
                                    //for (var j = 0; j < $scope.ActiveRolesListModel.length; j++) {
                                    //    if ($scope.EmployeesModel.UserTypeID == $scope.ActiveRolesListModel[i].UserTypeID) {
                                    //        if ($scope.ActiveRolesListModel[i].UserTypeName == 'Super Administrator'
                                    //            || $scope.ActiveRolesListModel[i].UserTypeName == 'Administrator') {
                                    //            $scope.required = false;
                                    //        }
                                    //        else {
                                    //            $scope.required = true;
                                    //        }
                                    //    }
                                    //}



                                    //if (data[i]["UserTypeName"] == "Supervisor") {
                                    //    var locids = data[i]["AssignedLocationID"].split(',');
                                    //    $scope.EmployeesModel.AssignedLocationID = locids;
                                    //    $scope.EmployeesModel.AssignedLotID = '';
                                    //}
                                    //else {

                                    //    $scope.EmployeesModel.AssignedLocationID = data[i]["AssignedLocationID"];
                                    //    GetActiveLotssList(data[i]["AssignedLocationID"]);
                                    //    $scope.EmployeesModel.AssignedLotID = data[i]["AssignedLotID"];
                                    //}

                                    if (data[i]["Photo"] != '') {
                                        $("#EmployeeImg").attr('src', 'EmployeeImages/' + data[i]["Photo"]);
                                       // $scope.EmployeesModel.EmpPhoto = data[i]["Photo"];
                                        if (!UrlExists('EmployeeImages/' + data[i]["Photo"])) {
                                            $("#EmployeeImg").attr('src', 'assets/images/picture-upload.jpg');
                                        }
                                    }
                                    else {
                                        $("#EmployeeImg").attr('src', 'assets/images/picture-upload.jpg');
                                    }
                                    if (data[i]["AadharPhoto"] != '') {
                                        $("#AadharImg").attr('src', 'EmployeeImages/' + data[i]["AadharPhoto"]);
                                        if (!UrlExists('EmployeeImages/' + data[i]["AadharPhoto"])) {
                                            $("#AadharImg").attr('src', 'assets/images/aadhaar-upload.jpg');
                                        }
                                    }
                                    else {
                                        $("#AadharImg").attr('src', 'assets/images/aadhaar-upload.jpg');
                                    }
                                    if (data[i]["PANPhoto"] != '') {
                                        $("#PANImg").attr('src', 'EmployeeImages/' + data[i]["PANPhoto"]);
                                        if (!UrlExists('EmployeeImages/' + data[i]["PANPhoto"])) {
                                            $("#PANImg").attr('src', 'assets/images/pan-upload.jpg');
                                        }
                                    }
                                    else {
                                        $("#PANImg").attr('src', 'assets/images/pan-upload.jpg');
                                    }
                                    $scope.$apply();//16122020
                                }
                            }
                            $('#loader-container').hide();
                        },
                        error: function (data) {
                            $('#loader-container').hide();
                        }
                    });
                }
                else {
                    window.location.href = $("#LogOut").val();
                }
            }
        }

        $scope.AssignStationtoUser = function () {
            var url_UserStation = $("#AssignStationtoUser").val();
            $scope.UserLocationMapperModel.UserID = $scope.EmployeesModel.UserID;
            // $scope.UserLocationMapperModel.UserID = 35;

            var userTypeName;

            for (var i = 0; i < $scope.ActiveRolesListModel.length; i++) {
                if ($scope.EmployeesModel.UserTypeID == $scope.ActiveRolesListModel[i].UserTypeID) {
                    userTypeName = $scope.ActiveRolesListModel[i].UserTypeName;
                }
            }

            var UserLocationMapperID;
            if ($scope.UserLocationMapperModel.UserLocationMapperID == "") {
                UserLocationMapperID = 0;
            }
            else {
                UserLocationMapperID = $scope.UserLocationMapperModel.UserLocationMapperID;
            }
            $scope.UserLocationMapperModel.UserLocationMapperID = UserLocationMapperID;

            if ($scope.UserLocationMapperModel.UserID != "" && $scope.UserLocationMapperModel.UserID != undefined) {
                if ($scope.UserLocationMapperModel.LocationParkingLotID != "" && $scope.UserLocationMapperModel.LocationParkingLotID != undefined) {


                    var url = $("#CheckLocationExistforReportToEmployee").val();
                    var reportToEmp = $scope.EmployeesModel.SupervisorID;
                    var locationid = $scope.UserLocationMapperModel.LocationID;

                    if (userTypeName != 'Super Administrator' && userTypeName != 'Administrator') {
                        $.ajax({
                            type: "POST",
                            url: url,
                            data: "{'SupervisorID':'" + reportToEmp + "','LocationID':'" + locationid + "'}",
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            success: function (data) {
                                if (data == true) {

                                    $.ajax({
                                        type: "POST",
                                        url: url_UserStation,
                                        data: "{'assignstationdata':" + JSON.stringify($scope.UserLocationMapperModel) + "}",
                                        contentType: "application/json; charset=utf-8",
                                        dataType: "json",
                                        success: function (data) {
                                            if (data == "Success") {
                                                GetAssignStationList($scope.UserLocationMapperModel.UserID);
                                                $scope.UserLocationMapperModel = {
                                                    'UserLocationMapperID': '',
                                                    'UserID': '',
                                                    'UserName': '',
                                                    'LocationID': '',
                                                    'LocationName': '',
                                                    'LocationParkingLotID': '',
                                                    'LocationParkingLotName': '',
                                                    'IsActive': '',
                                                    'IsDeleted': '',
                                                    'UpdatedBy': '',
                                                    'UpdatedOn': ''
                                                };
                                                $scope.$apply();
                                            }
                                            else if (data == "Data Exists") {
                                                alert("Location/Lot already assign to this User exist.");
                                            }
                                        },
                                        error: function (data) {
                                        }
                                    });


                                    $('#loader-container').hide();

                                }
                                else {
                                    alert('Reporting Manager doesnt have the Location which you are trying to Assign.');
                                    $('#loader-container').hide();
                                }
                            },
                            error: function (data) {

                                $('#loader-container').hide();

                            }
                        });
                    }
                    else {
                        $.ajax({
                            type: "POST",
                            url: url_UserStation,
                            data: "{'assignstationdata':" + JSON.stringify($scope.UserLocationMapperModel) + "}",
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            success: function (data) {
                                if (data == "Success") {
                                    GetAssignStationList($scope.UserLocationMapperModel.UserID);
                                    $scope.UserLocationMapperModel = {
                                        'UserLocationMapperID': '',
                                        'UserID': '',
                                        'UserName': '',
                                        'LocationID': '',
                                        'LocationName': '',
                                        'LocationParkingLotID': '',
                                        'LocationParkingLotName': '',
                                        'IsActive': '',
                                        'IsDeleted': '',
                                        'UpdatedBy': '',
                                        'UpdatedOn': ''
                                    };
                                    $scope.$apply();
                                }
                                else if (data == "Data Exists") {
                                    alert("Location/Lot already assign to this User exist.");
                                }
                            },
                            error: function (data) {
                            }
                        });
                    }

                }
                else {
                    alert('Please select Lot.')
                }

            }
            else {
                alert('you need to create Employee');
            }
        };
        function GetAssignStationList(userid) {
            var url = $("#GetAssignStationList").val();
            if (url != undefined) {
                $('#loader-container').show();
                if (CheckInSession()) {
                    $.ajax({
                        type: "POST",
                        url: url,
                        data: "{'UserID':'" + userid + "'}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            if (data.length > 0) {
                                $scope.UserLocationMapperListModel = data;
                                for (var items = 0; items < $scope.UserLocationMapperListModel.length; items++) {
                                    if ($scope.UserLocationMapperListModel[items].IsActive == true) {
                                        $scope.UserLocationMapperListModel[items].IsActive = 'Active';
                                    }
                                    else {
                                        $scope.UserLocationMapperListModel[items].IsActive = 'Inactive';
                                    }
                                }
                                $scope.$apply();//16122020
                            }
                            $('#loader-container').hide();
                        },
                        error: function (data) {
                            $('#loader-container').hide();
                        }
                    });
                }
                else {
                    window.location.href = $("#LogOut").val();
                }
            }
        }
        $scope.EditAssignStation = function (userlocationid) {
            var url = $("#GetAssignStationByID").val();
            var hdnFlagVal = userlocationid;

            if (hdnFlagVal != undefined && url != undefined) {
                $('#loader-container').show();
                if (CheckInSession()) {
                    $.ajax({
                        type: "POST",
                        url: url,
                        data: "{'UserLocationMapperID':" + hdnFlagVal + "}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            if (data.length > 0) {
                                for (var i = 0; i < data.length; i++) {
                                    $scope.UserLocationMapperModel.UserLocationMapperID = data[i]["UserLocationMapperID"];
                                    $scope.UserLocationMapperModel.UserID = data[i]["UserID"];
                                    $scope.UserLocationMapperModel.LocationID = data[i]["LocationID"];
                                    GetActiveLotssList(data[i]["LocationID"]);
                                    $scope.UserLocationMapperModel.LocationParkingLotID = data[i]["LocationParkingLotID"];
                                    $scope.UserLocationMapperModel.IsActive = data[i]["IsActive"];
                                    $scope.$apply();
                                }
                            }
                            $('#loader-container').hide();
                        },
                        error: function (data) {
                            $('#loader-container').hide();
                        }
                    });
                }
                else {
                    window.location.href = $("#LogOut").val();
                }
            }
        };
        $scope.DeleteAssignStation = function (userlocationid) {
            var url = $("#DeleteAssignStation").val();
            var hdnFlagVal = userlocationid;

            var hdnUpdateUserID = $("#hdnUpdateUserID").val();
            if (hdnUpdateUserID != "" && hdnUpdateUserID != undefined) {
                $scope.UserLocationMapperModel.UserID = hdnUpdateUserID;
            } else {
                $scope.UserLocationMapperModel.UserID = $scope.EmployeesModel.UserID;
            }


            if (hdnFlagVal != undefined && url != undefined) {
                $('#loader-container').show();
                if (CheckInSession()) {
                    $.ajax({
                        type: "POST",
                        url: url,
                        data: "{'UserLocationID':" + hdnFlagVal + "}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            if (data != "Failed") {
                                GetAssignStationList($scope.UserLocationMapperModel.UserID);
                            }
                            $('#loader-container').hide();
                        },
                        error: function (data) {
                            $('#loader-container').hide();
                        }
                    });
                }
                else {
                    window.location.href = $("#LogOut").val();
                }
            }
        };

        function EditEmployeeByID() {
            var url = $("#ViewEmployee").val();
            var hdnFlagVal = $stateParams.employeeid;

            if (hdnFlagVal != undefined && url != undefined) {
                $("input[id=hdnUpdateUserID]").val(hdnFlagVal);

                //New
                GetSavedUserDetailsByID(hdnFlagVal);
                GetAssignStationList(hdnFlagVal);
                //New



                //$('#loader-container').show();
                //if (CheckInSession()) {

                //    $.ajax({
                //        type: "POST",
                //        url: url,
                //        data: "{'EmployeeID':" + hdnFlagVal + "}",
                //        contentType: "application/json; charset=utf-8",
                //        dataType: "json",
                //        success: function (data) {
                //            if (data.length > 0) {
                //                for (var i = 0; i < data.length; i++) {
                //                    $scope.EmployeesModel.UserID = data[i]["UserID"];
                //                    $scope.EmployeesModel.UserTypeID = data[i]["UserTypeID"];
                //                    $scope.EmployeesModel.UserCode = data[i]["UserCode"];
                //                    $scope.EmployeesModel.UserName = data[i]["UserName"];
                //                    $scope.EmployeesModel.Password = data[i]["Password"];
                //                    $scope.EmployeesModel.PhoneNumber = data[i]["PhoneNumber"];
                //                    $scope.EmployeesModel.SupervisorID = data[i]["SupervisorID"];
                //                    $scope.EmployeesModel.IsActive = data[i]["IsActive"];

                //                    $scope.EmployeesModel.JoiningDate = new Date(data[i]["JoiningDate"]);
                //                    $scope.EmployeesModel.Salary = data[i]["Salary"];
                //                    $scope.EmployeesModel.EPFAccountNumber = data[i]["EPFAccountNumber"];
                //                    $scope.EmployeesModel.AltPhoneNumber = data[i]["AltPhoneNumber"];
                //                    $scope.EmployeesModel.Address = data[i]["Address"];
                //                    $scope.EmployeesModel.AadharNumber = data[i]["AadharNumber"];
                //                    $scope.EmployeesModel.PANNumber = data[i]["PANNumber"];                                   

                //                    if (data[i]["Photo"] != '') {
                //                        $("#EmployeeImg").attr('src', data[i]["Photo"]);
                //                    }
                //                    else {
                //                        $("#EmployeeImg").attr('src', '../Images/default-logo.png');
                //                    }
                //                    if (data[i]["AadharPhoto"] != '') {
                //                        $("#AadharImg").attr('src', data[i]["AadharPhoto"]);
                //                    }
                //                    else {
                //                        $("#AadharImg").attr('src', '../Images/default-logo.png');
                //                    }
                //                    if (data[i]["PANPhoto"] != '') {
                //                        $("#PANImg").attr('src', data[i]["PANPhoto"]);
                //                    }
                //                    else {
                //                        $("#PANImg").attr('src', '../Images/default-logo.png');
                //                    }

                //                    //$scope.CustomerModel.ExpiryDate = new Date(data[i]["ExpiryDate"]);
                //                    //$('#ExpiryDate').val(data[i]["ExpiryDate"])[0].value;
                //                    //$scope.CustomerModel.ExpiryDate = $scope.CustomerModel.ExpiryDate.toLocaleDateString();

                //                    $scope.$apply();
                //                }
                //            }
                //            $('#loader-container').hide();
                //        },
                //        error: function (data) {
                //            $('#loader-container').hide();
                //        }
                //    });
                //}
                //else {
                //    window.location.href = $("#LogOut").val();
                //}
            }
        }
        $scope.UpdateEmployee = function () {

            var filesizeflag = true;

            var fdata = new FormData();
            var photoLogo = $("#Pictureupload").get(0);
            var files1 = photoLogo.files;
            if (files1.length > 0) {
                for (var i = 0; i < files1.length; i++) {
                    fdata.append("EmpPhotoLogo", files1[i]);
                }
                //var photosize = $('#Pictureupload')[0].files[0].size;
                //if (photosize > 20480) {
                //    filesizeflag = false;
                //}
            }
            

            var AadharLogo = $("#Aadharupload").get(0);
            var files2 = AadharLogo.files;
            if (files2.length > 0) {
                for (var j = 0; j < files2.length; j++) {
                    fdata.append("AadharProof", files2[j]);
                }
                //var aadharsize = $('#Aadharupload')[0].files[0].size;
                //if (aadharsize > 20480) {
                //    filesizeflag = false;
                //}
            }
            var PANLogo = $("#Panupload").get(0);
            var files3 = PANLogo.files;
            if (files3.length > 0) {
                for (var k = 0; k < files3.length; k++) {
                    fdata.append("PANProof", files3[k]);
                }
                //var PANsize = $('#Panupload')[0].files[0].size;
                //if (PANsize > 20480) {
                //    filesizeflag = false;
                //}
            }

            var date;

            var url = $("#UpdateEmployee").val();
            $scope.EmployeesModel.UserID = $stateParams.employeeid;

            if (url != undefined) {
                $('#loader-container').show();
                if (CheckInSession()) {
                    //  if (filesizeflag) {

                    if (typeof $scope.EmployeesModel.JoiningDate == "string") {
                        date = $scope.EmployeesModel.JoiningDate;
                    }
                    else {
                        date = $scope.EmployeesModel.JoiningDate.toLocaleDateString();
                    }
                    $scope.EmployeesModel.JoiningDate = date;

                    $.ajax({
                        type: "POST",
                        url: url,
                        data: "{'UsersData':" + JSON.stringify($scope.EmployeesModel) + "}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            if (data != "Failed") {

                                var sucessdata = data;
                                var profidenity;
                                if (sucessdata.length > 0) {
                                    profidenity = sucessdata.split('@')[1];
                                }

                                $scope.EmployeesModel.UserID = profidenity;

                                fdata.append('EmployeeDetails', JSON.stringify($scope.EmployeesModel));

                                var options = {};
                                options.url = "Handlers/EmployeeFileHandler.ashx";
                                options.type = "POST";
                                options.data = fdata;
                                options.contentType = false;
                                options.processData = false;
                                options.success = function (result) {

                                };
                                options.error = function (err) {
                                };

                                $.ajax(options);

                                alert("Employee Updated Successfully");
                                GetSavedUserDetailsByID($scope.EmployeesModel.UserID);
                            }
                            $('#loader-container').hide();
                        },
                        error: function (data) {
                            $('#loader-container').hide();
                        }
                    });

                    //}
                    //else {
                    //    alert('Images Size should be less than or equal to 20KB');
                    //    $('#loader-container').hide();
                    //}
                }
                else {
                    window.location.href = $("#LogOut").val();
                }
            }
        };

        $scope.OpenPopup = function (userTypeID) {
            GetModulesSubModulesList(userTypeID);
            $("input[id=hdnUserTypeID]").val(userTypeID);
            $('#modal').modal('toggle');
            $('#myModal').modal('show');
            $scope.$apply();
        };
        function GetModulesSubModulesList(usertypeID) {
            var url = $("#GetModulesSubModulesList").val();
            var hdnFlagVal = usertypeID;
            if (url != undefined) {
                $('#loader-container').show();
                if (CheckInSession()) {
                    $.ajax({
                        type: "POST",
                        url: url,
                        data: "{'UserTypeID':" + hdnFlagVal + "}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            if (data.length > 0) {
                                $scope.ModulesList = data;
                                $scope.$apply();
                                $('#loader-container').hide();
                            }
                        },
                        error: function (data) {
                            $('#loader-container').hide();
                        }
                    });
                }
                else {
                    window.location.href = $("#LogOut").val();
                }
            }
        }
        $scope.isSelectAll = function (modules) {
            if (!modules.IsAssign) {
                for (var j = 0; j < modules.SubModules.length; j++) {
                    modules.SubModules[j].IsAssign = false;
                }
            }
            else {
                for (var i = 0; i < modules.SubModules.length; i++) {
                    modules.SubModules[i].IsAssign = true;
                }
            }
            $scope.$apply();
        }
        $scope.isSelect = function (modules, submodule) {

            var allselected = false;
            for (var i = 0; i < modules.SubModules.length; i++) {
                if (modules.SubModules[i].IsAssign == true) {
                    allselected = true;
                }
            }
            modules.IsAssign = allselected;
            $scope.$apply();
        };
        $scope.AssignModule = function () {
            var url = $("#AssignModules").val();
            var usertypeID = $("#hdnUserTypeID").val();

            for (var i = 0; i < $scope.ModulesList.length; i++) {
                for (var j = 0; j < $scope.ModulesList[i].SubModules.length; j++) {
                    if ($scope.ModulesList[i].SubModules[j].IsAssign == 'Yes') {
                        $scope.ModulesList[i].SubModules[j].IsAssign = true;
                    }
                    else if ($scope.ModulesList[i].SubModules[j].IsAssign == 'No') {
                        $scope.ModulesList[i].SubModules[j].IsAssign = false;
                    }
                }
            }

            $('#loader-container').show();
            if (CheckInSession()) {

                $.ajax({
                    type: "POST",
                    url: url,
                    data: "{'modulesArray':" + JSON.stringify($scope.ModulesList) + ",'userTypeID':'" + usertypeID + "'}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {
                        if (data != "Failed") {
                            //GetModulesSubModulesList(userID);
                            //$('#myModal').modal('hide');
                        }
                        $('#loader-container').hide();
                    },
                    error: function (data) {
                        $('#loader-container').hide();
                    }
                });
            }
            else {
                window.location.href = $("#LogOut").val();
            }
        };

        

        //Employees Code End

        GetActiveLocationsList();
        function GetActiveLocationsList() {
            var url = $("#GetActiveLocationsList").val();
            if (url != undefined) {
                $('#loader-container').show();
                if (CheckInSession()) {
                    $.ajax({
                        type: "POST",
                        url: url,
                        data: "",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            if (data.length > 0) {
                                $scope.GetActiveLocationListModel = data;
                                $scope.$apply();
                            }
                            $('#loader-container').hide();
                        }, error: function (data) {
                            $('#loader-container').hide();
                        }
                    });
                }
                else {
                    window.location.href = $("#LogOut").val();
                }
            }
        }
        function GetActiveLotssList(LocationID) {
            $scope.GetActiveLotListModel = [];
            var url = $("#GetActiveLotssList").val();
            if (url != undefined) {
                $('#loader-container').show();
                if (CheckInSession()) {
                    $.ajax({
                        type: "POST",
                        url: url,
                        data: "{'LocationID':" + LocationID + "}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            if (data.length > 0) {
                                $scope.GetActiveLotListModel = data;
                                $scope.$apply();
                            }
                            $('#loader-container').hide();
                        }, error: function (data) {
                            $('#loader-container').hide();
                        }
                    });
                }
                else {
                    window.location.href = $("#LogOut").val();
                }
            }
        }
        $scope.GetActiveLots = function (LocationID) {
            GetActiveLotssList(LocationID);
        };

        //08122020 Start
        GetEmployeeProfileDetails();
        function GetEmployeeProfileDetails() {
            var url = $("#ViewEmployeeProfile").val();
            var hdnFlagVal = $("#hdnUpdateUserID").val();
            if (hdnFlagVal != undefined && url != undefined) {
                $('#loader-container').show();
                if (CheckInSession()) {

                    $.ajax({
                        type: "POST",
                        url: url,
                        data: "{'EmployeeID':" + hdnFlagVal + "}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            if (data.length > 0) {
                                for (var i = 0; i < data.length; i++) {
                                    $scope.EmployeesModel.UserID = data[i]["UserID"];
                                    $scope.EmployeesModel.UserTypeID = data[i]["UserTypeID"];
                                    $scope.EmployeesModel.UserTypeName = data[i]["UserTypeName"];
                                    $scope.EmployeesModel.UserCode = data[i]["UserCode"];
                                    $scope.EmployeesModel.UserName = data[i]["UserName"];
                                    $scope.EmployeesModel.Password = data[i]["Password"];
                                    $scope.EmployeesModel.PhoneNumber = data[i]["PhoneNumber"];
                                    $scope.EmployeesModel.SupervisorID = data[i]["SupervisorID"];
                                    $scope.EmployeesModel.IsActive = data[i]["IsActive"];

                                    $scope.EmployeesModel.JoiningDate = new Date(data[i]["JoiningDate"]);
                                    $scope.EmployeesModel.Salary = data[i]["Salary"];
                                    $scope.EmployeesModel.EPFAccountNumber = data[i]["EPFAccountNumber"];
                                    $scope.EmployeesModel.AltPhoneNumber = data[i]["AltPhoneNumber"];
                                    $scope.EmployeesModel.Address = data[i]["Address"];
                                    $scope.EmployeesModel.AadharNumber = data[i]["AadharNumber"];
                                    $scope.EmployeesModel.PANNumber = data[i]["PANNumber"];
                                    $scope.$apply();
                                }
                            }
                            $('#loader-container').hide();
                        },
                        error: function (data) {
                            $('#loader-container').hide();
                        }
                    });
                }
                else {
                    window.location.href = $("#LogOut").val();
                }
            }
        }
        //08122020 End

        // table code

        $scope.selected = [];
        $scope.limitOptions = [10, 20, 30];

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
            limit: 10,
            page: 1
        };

        $scope.editComment = function (event, zonedata) {
            event.stopPropagation(); // in case autoselect is enabled            

            var promise;

            if ($scope.options.largeEditDialog) {
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
            $scope.limitOptions = $scope.limitOptions ? undefined : [10, 20,30];
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

    }

    function CheckInSession() {
        var url = $("#CheckSessionValue").val();
        var flagVal = false;
        // $("#loadingDiv").show();

        $.ajax({
            type: "POST",
            contentType: "application/json;",
            url: url,
            data: "{}",
            async: false,
            dataType: "text",
            success: function (data) {
                if (data == 'true') {
                    flagVal = true;
                }
                else {
                    flagVal = false;
                    // window.location.href = $("#LogOut").val();
                }
            }, error: function (xhr) {
                flagVal = false;
            }
        });

        // alert(flagVal)
        return flagVal;
    }

    angular.module('app').directive('allowNumbers', function () {
        return {
            require: 'ngModel',
            link: function (scope, element, attr, ngModelCtrl) {
                function fromUser(text) {
                    if (text) {
                        var transformedInput = text.replace(/[^0-9]/g, '');

                        if (transformedInput !== text) {
                            ngModelCtrl.$setViewValue(transformedInput);
                            ngModelCtrl.$render();
                        }
                        return transformedInput;
                    }
                    return undefined;
                }
                ngModelCtrl.$parsers.push(fromUser);
            }
        };
    });

    angular.module('app').directive('validNumbers', function () {
        return {
            require: '?ngModel',
            link: function (scope, element, attrs, ngModelCtrl) {
                if (!ngModelCtrl) {
                    return;
                }

                ngModelCtrl.$parsers.push(function (val) {
                    if (angular.isUndefined(val)) {
                        var val = '';
                    }

                    var clean = val.replace(/[^-0-9\.]/g, '');
                    var negativeCheck = clean.split('-');
                    var decimalCheck = clean.split('.');
                    if (!angular.isUndefined(negativeCheck[1])) {
                        negativeCheck[1] = negativeCheck[1].slice(0, negativeCheck[1].length);
                        clean = negativeCheck[0] + '-' + negativeCheck[1];
                        if (negativeCheck[0].length > 0) {
                            clean = negativeCheck[0];
                        }
                    }

                    if (!angular.isUndefined(decimalCheck[0])) {
                        decimalCheck[0] = decimalCheck[0].slice(0, 6);
                        clean = decimalCheck[0];
                    }

                    if (!angular.isUndefined(decimalCheck[1])) {
                        decimalCheck[1] = decimalCheck[1].slice(0, 2);
                        clean = decimalCheck[0] + '.' + decimalCheck[1];
                    }

                    if (val !== clean) {
                        ngModelCtrl.$setViewValue(clean);
                        ngModelCtrl.$render();
                    }
                    return clean;
                });

                element.bind('keypress', function (event) {
                    if (event.keyCode === 32) {
                        event.preventDefault();
                    }
                });
            }
        };
    });

    angular.module('app').directive('capitalize', function () {
        return {
            require: 'ngModel',
            link: function (scope, element, attrs, modelCtrl) {
                var capitalize = function (inputValue) {
                    if (inputValue == undefined) inputValue = '';
                    var capitalized = inputValue.toUpperCase();
                    if (capitalized !== inputValue) {
                        // see where the cursor is before the update so that we can set it back
                        var selection = element[0].selectionStart;
                        modelCtrl.$setViewValue(capitalized);
                        modelCtrl.$render();
                        // set back the cursor after rendering
                        element[0].selectionStart = selection;
                        element[0].selectionEnd = selection;
                    }
                    return capitalized;
                };
                modelCtrl.$parsers.push(capitalize);
                capitalize(scope[attrs.ngModel]); // capitalize initial value
            }
        };
    });

    angular.module('app').directive('readonly', function () {
        return {
            restrict: 'EAC',
            link: function (scope, elem, attr) {
               // $('#JoiningDate').attr('readonly', true);
                document.querySelectorAll("#JoiningDate input")[0].setAttribute("readonly", "readonly");
                angular.element(".md-datepicker-button").each(function () {
                    var el = this;
                    var ip = angular.element(el).parent().find("input").bind('click', function (e) {
                        angular.element(el).click();
                    });
                    angular.element(this).css('display', 'none');
                });
            }
        };
    });

})(); 
