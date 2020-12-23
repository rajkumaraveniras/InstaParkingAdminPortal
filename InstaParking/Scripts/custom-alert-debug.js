/**
 * customalerts.js
 * Author: Philippe Assis
 * Doc and repo: https://github.com/PhilippeAssis/custom-alert
 *
 * Alert e confirm personalizados.
 * FF, Chromer, IE(>=9)*
 *
 *                              ATENÇÂO
 * window.customalert e window.customconfirm devem permanecer com esses nomes,
 * a não ser que você saiba o que esta fazendo.
 */



function customAlert(inGlobalVar) {

    

    function mergeObjects(obj1, obj2) {
        for (var key in obj2) {
            obj1[key] = obj2[key];
        }

        return obj1;
    }

    function Alert() {
        var AlertDefaultOptions = {
            'button': 'OK',
            'title': ''
        };

        this.render = function (dialog, options) {
            if (options) {
                this.options = mergeObjects(AlertDefaultOptions, options);
            }
            else {
                this.options = AlertDefaultOptions;
            }

            var alertBox = $(document).find("#customalert");
            alertBox.find(".header").html(this.options.title);
            alertBox.find(".body").html(dialog);
            alertBox.find(".button-done").html(this.options.button);
            $(document).find("html").css({ "overflow": "hidden" });
            $(document).find("#customalert-overlay").show()//.css({ "display": "block" })//.style.display = "block";
            alertBox.show()//.css({ "display": "block" });//.style.display = "block";
        };

        this.done = function () {
            $(document).find("#customalert").hide().css({ "display": "none" })//.style.display = null;
            $(document).find("#customalert-overlay").hide().css({ "display": "none" })//.style.display = null;
            $(document).find("html").css({ "overflow": "auto" })//.style.overflow = "auto";

            if (typeof this.callback == 'function') {
                this.callback.call()
            }
        }
    }

    function Confirm() {
        var confirmDefaultOptions = {
            "done": {
                "text": "Ok",
                "bold": false,
                "default": true
            },
            "cancel": {
                "text": "Cancel",
                "bold": false,
                "default": false
            },
            'title': ''
        };

        var getText = function (options, obj) {
            if (options[obj].bold) {
                return "<strong>" + options[obj].text + "</strong>"
            }

            return options[obj].text
        }

        this.callback = function (data) { };

        this.render = function (dialog, options) {
            this.options = confirmDefaultOptions;

            if (options) {
                if (options.done && typeof options.done == "string") {
                    options.done = {
                        "text": options.done
                    }
                }

                if (options.cancel && typeof options.cancel == "string") {
                    options.cancel = {
                        "text": options.cancel
                    }
                }

                if (options.cancel.default == true) {
                    options.done.default = false;
                }
                else {
                    options.done.default = true;
                }

                console.log(confirmDefaultOptions)
                if (options.cancel) {
                    this.options.cancel = mergeObjects(confirmDefaultOptions.cancel, options.cancel)
                }
                if (options.done) {
                    this.options.done = mergeObjects(confirmDefaultOptions.done, options.done)
                }
                if (options.title) {
                    this.options.title = confirmDefaultOptions.title
                }
            }

            var confirmBox = $(document).find("#customconfirm");
            confirmBox.find(".header").html(this.options.title);
            confirmBox.find(".body").html(dialog);
            confirmBox.find(".button-cancel").html(getText(this.options, "cancel"));
            confirmBox.find(".button-done").html(getText(this.options, "done"));
            $(document).find("html").css({ "overflow": "hidden" })//.style.overflow = "hidden";
            $(document).find("#customconfirm-overlay").show();//.css({ "display": "block" })//.style.display = "block";
            confirmBox.show()//.css({ "display": "block" })//.style.display = "block";
        };

        this.done = function () {
            this.end();

            if (this.callbackSuccess) {
                return this.callbackSuccess();
            }

            this.callback(true);
        }

        this.cancel = function () {
            this.end();

            if (this.callbackCancel) {
                return this.callbackCancel();
            }

            this.callback(false);
        }

        this.end = function () {
            $(document).find("#customconfirm").hide();//.css({ "display": "none" })//.style.display = "none";
            $(document).find("#customconfirm-overlay").hide();//.css({ "display": "none" })//.style.display = "none";
            $(document).find("html").css({ "overflow": "auto" })//.style.overflow = "auto";
        }
    }

    var cAlert, cConfirm;

    if (document.getElementById("customalert") == null) {


        $('body').append('<div id="customalert-overlay" class="customalert-overlay"></div>')
        
        $('body').append('<div id="customalert" class="customalert customalert-alert"></div>')
       
        $('#customalert').append('<div class="header"></div>')
        
        $('#customalert').append('<div class="body"></div>')
        
        $('#customalert').append('<div class="footer"></div>')
       
        $('#customalert .footer').append('<button class="btn btn-primary custom-alert button-done" onclick="window.customalert.done()"></button>')
       
        
        cAlert = window.Alert = function (dialog, options, callback) {
            window.customalert = new Alert();
            if (typeof options == 'function') {
                window.customalert.callback = options;
                options = null
            }
            else {
                window.customalert.callback = callback || null;
            }

            window.customalert.render(dialog, options);
        };
    }

    if (document.getElementById("customconfirm") == null) {
        
        $('body').append('<div id="customconfirm-overlay" class="customconfirm-overlay"></div>')

      
        $('body').append('<div id="customconfirm" class="customalert customalert-confirm"></div>')
       
        $('#customconfirm').append('<div class="header"></div>')

       
        $('#customconfirm').append('<div class="body"></div>')
       
        $('#customconfirm').append('<div class="footer"></div>')
       

        $('#customconfirm .footer').append('<button class="btn btn-success custom-alert button-done" onclick="window.customconfirm.done()"></button>')

       
        $('#customconfirm .footer').append('<button class="btn btn-danger button-cancel" onclick="window.customconfirm.cancel()"></button>')
       

        cConfirm = window.Confirm = function (dialog, callback, options) {
            window.customconfirm = new Confirm();
            if (typeof callback == 'object') {
                window.customconfirm.callbackSuccess = callback.success;
                window.customconfirm.callbackCancel = callback.cancel;
            }
            else {
                window.customconfirm.callback = callback;
            }

            window.customconfirm.render(dialog, options);
        };

    }


    if (inGlobalVar === false) {
        return {
            "alert": cAlert,
            "confirm": cConfirm
        }
    }
    else {
        window.alert = cAlert
        window.confirm = cConfirm
    }
}
