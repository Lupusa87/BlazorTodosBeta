
window.BTodosJSFunctions = {
    Alert: function (msg) {
        alert(msg);
        return true;
    },
    Log: function (msg) {
        console.log(msg);
        return true;
    },
    GetTimezoneOffset: function () {
        return new Date().getTimezoneOffset()/60;
    },
    GetDateMilliseconds: function () {
        return new Date().getTime();
    },
    GetMachineID: function () {
        return GetFingerprint().toString();
    },
    GetElementActualWidth: function (el) {
        if (document.getElementById(el) !== null) {
            let rect = document.getElementById(el).getBoundingClientRect();
            return rect.width;
        }
        else {
            return 0;
        }
    },
    GetElementActualHeight: function (el) {

        if (document.getElementById(el) !== null) {
            let rect = document.getElementById(el).getBoundingClientRect();
            return rect.height;
        }
        else {
            return 0;
        }
    },
    GetElementActualTop: function (el) {
        if (document.getElementById(el) !== null) {
            let rect = document.getElementById(el).getBoundingClientRect();
            return rect.top;
        }
        else {
            return 0;
        }
    },
    GetElementActualLeft: function (el) {
        if (document.getElementById(el) !== null) {
            let rect = document.getElementById(el).getBoundingClientRect();
            return rect.left;
        }
        else {
            return 0;
        }
    },
    GetWindowWidth: function () {
        return window.innerWidth
            || document.documentElement.clientWidth
            || document.body.clientWidth;

    },
    GetWindowHeight: function () {
        return window.innerHeight
            || document.documentElement.clientHeight
            || document.body.clientHeight;

    }
}