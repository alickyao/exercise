﻿/*
*** EasyUi表单验证扩展
*/

$(document).ready(function () {
    var ValidateboxRetrunMessage = "";
    $.extend($.fn.validatebox.defaults.rules, {
        mobile: {//检查手机号码
            validator: function (value, param) {
                var regx = /^[0-9]{11}$/;
                var flag = regx.test(value);
                return flag;
            },
            message: '请填写正确的手机号码'
        },
        idcard: {//检查身份证号码
            validator: function (value, param) {
                var idtype = $(param[0]).combobox("getValue");
                if (idtype == 0) {
                    var regx = /(^\d{18}$)|(^\d{17}(\d|X|x)$)/;
                    var flag = regx.test(value);
                    return flag;
                }
                else {
                    return true;
                }
            },
            message: '请填写正确的身份证号码'
        },
        orgcaption: {//检查组织名称
            validator: function (value, param) {
                var r = false;
                if (value.length < 2 || value.length > 25) {
                    r = false;
                    return r;
                }
                $.ajax({
                    url: "/api/ApiDepartments/GetDepartmentCapitonNum?caption=" + value + "&orgid=" + param[0],
                    async: false,
                    dataType: 'json',
                    type: 'get',
                    contentType: "application/json",
                    success: function (data) {
                        r = (data == 0);
                    }
                });
                return r;
            },
            message: "该组织名称已经被使用,或字符太少或者太多"
        },
        checkusermobilephone: {
            validator: function (value, param) {
                var r = false;
                $.ajax({
                    url: "/api/ApiMembers/CheckMobilePhoneisInUse?mobilePhone=" + value,
                    async: false,
                    dataType: 'json',
                    type: 'get',
                    contentType: "application/json",
                    success: function (data) {
                        r = (data == 0);
                    }
                });
                return r;
            },
            message: "该手机号码已被使用"
        },
        checkdateforend: {//比较两个时间
            validator: function (value, param) {
                var begindate = new Date($(param[0]).val());
                var enddate = new Date(value);
                return enddate > begindate; //结束时间必须大于开始时间
            },
            message: "所选时间不能早于起始时间"
        },
        checkdateforbegin: {//比较两个时间
            validator: function (value, param) {
                var enddate = new Date($(param[0]).val());
                var begindate = new Date(value);
                return enddate > begindate; //结束时间必须大于开始时间
            },
            message: "所选时间不能晚于结束时间"
        },
        checkthisdate: {//检查日期是否大于指定的天数
            validator: function (value, param) {
                var curDate = new Date();
                var newDate = new Date(curDate.setDate(curDate.getDate() + (param[0]-1)));
                var Reg = true;
                var vDate = new Date(value);
                if (vDate < newDate) {
                    Reg = false;
                }
                return Reg;
            },
            message:"不能小于指定的天数"
        }
    });
});