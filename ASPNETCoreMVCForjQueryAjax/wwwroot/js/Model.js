class Model {

    static Alert(value) {
        alert(value);
    }

    static GetURL()
    {
        return "https://localhost:44309";
    }

    static RequestToServer(json, url, actionAfterResponse, actionElementIDAfterResponse = "", callbackFun = null,
        httpType = "POST", contentType = "application/json; charset=utf-8", isCache = false, dataType = "json") {

        try {
            $.ajax({
                type: httpType,
                url: url,
                contentType: contentType,
                cache: isCache,
                data: json,
                dataType: dataType,
                success: getSuccess,
                error: getFail
            });
        } catch (e) {
            Model.Alert(e);
        }
        function getSuccess(data, textStatus, jqXHR) {

            switch (actionAfterResponse) {
                default:
                    callbackFun(data)
            }
        };
        function getFail(jqXHR, textStatus, errorThrown) {
            Model.Alert(jqXHR.status);
        };
    }

    static ShowSection(element) {

        try {
            switch (element) {
                case "View":
                    document.getElementById("view").style.display = "block";          
                    document.getElementById("add").style.display = "none";
                    document.getElementById("addFeedback").style.display = "none";
                    Model.InitializedView();
                    break;                               

                case "Add":
                    document.getElementById("view").style.display = "none";                 
                    document.getElementById("add").style.display = "block";
                    document.getElementById("addFeedback").style.display = "none";
                    Model.InitializedAdd()
                    break;

                case "AddFeedback":
                    document.getElementById("view").style.display = "none";                
                    document.getElementById("add").style.display = "none";
                    document.getElementById("addFeedback").style.display = "block";
                    Model.InitializedAddFeedback();
                    Model.PopulateRetrospectives();
                    Model.PopulateFeedbackTypes();                    
                    break;

                default:
                    break;
            }
        }
        catch (err) {
            Model.Alert(err.message)
        }
    }

    static InitializedAdd() {
        document.getElementById("result").innerHTML = null;
        document.getElementById("participants").value = null;
        document.getElementById("name").value = null;
        document.getElementById("summary").value = null;
        document.getElementById("date").value = null;
    }

    static Add() {
        try {
            var app = new Vue({
                el: '#add',
                data: {
                    appData: []
                },
                methods: {
                    fun: function (json) {
                        this.appData = json;
                        document.getElementById("result").innerHTML = this.appData.message
                    },

                    add: function () {

                        var textArea = document.getElementById("participants");
                        var lines = textArea.value.split("\n");

                        var data =
                        {
                            Name: document.getElementById("name").value, Summary: document.getElementById("summary").value,
                            Date: document.getElementById("date").value, ParticipantList: lines

                        };
                        var json = JSON.stringify(data);

                        Model.RequestToServer(json, Model.GetURL() + "/API/Add", "", "", this.fun)
                    }
                }
            })
        }
        catch (err) {
            Model.Alert(err.message)
        }
    }

    static InitializedView() {
        document.getElementById("fromDate").value = null;        
        document.getElementById("toDate").value = null;
    }

    static View() {
        try {
            var app = new Vue({
                el: '#view',
                data: {
                    appData: []
                },
                methods: {
                    fun: function (json) {
                        this.appData = json;
                    },

                    view: function () {

                        var data =
                        {
                            FromDate: document.getElementById("fromDate").value,
                            ToDate: document.getElementById("toDate").value
                        };
                        var json = JSON.stringify(data);

                        Model.RequestToServer(json, Model.GetURL() + "/API/View", "", "", this.fun)
                    }
                }
            })
        }
        catch (err) {
            Model.Alert(err.message)
        }
    }

    static PopulateRetrospectiveSelect(json) {

        var ele = document.getElementById('retrospectives');
        ele.innerHTML = '<option value=""></option>';
        for (var i = 0; i < json.length; i++) {

            ele.innerHTML = ele.innerHTML +
                '<option value="' + json[i]['name'] + '">' + json[i]['name'] + '</option>';
        }
    }

    static PopulateRetrospectives() {

        Model.Populate(Model.GetURL() + "/API/RetrospectiveNames", Model.PopulateRetrospectiveSelect)
    }

    static PopulateFeedbackTypeSelect(json) {

        var ele = document.getElementById('feedbackType');
        ele.innerHTML = '<option value=""></option>';
        for (var i = 0; i < json.length; i++) {

            ele.innerHTML = ele.innerHTML +
                '<option value="' + json[i]['name'] + '">' + json[i]['name'] + '</option>';
        }
    }

    static PopulateFeedbackTypes() {

        Model.Populate(Model.GetURL() + "/API/FeedbackTypes", Model.PopulateFeedbackTypeSelect)
    }

    static Populate(url, fun) {

        Model.RequestToServer('', url, "", "", fun)
    }

    static InitializedAddFeedback() {
        document.getElementById("addFeedbackResult").innerHTML = null;
        document.getElementById("retrospectives").value = null;
        document.getElementById("nameOfPerson").value = null;
        document.getElementById("body").value = null;
        document.getElementById("feedbackType").value = null;
    }

    static AddFeedback() {
        try {
            var app = new Vue({
                el: '#addFeedback',
                data: {
                    appData: [],
                    retrospectives: []
                },
                methods: {
                    fun: function (json) {
                        this.appData = json;
                        document.getElementById("addFeedbackResult").innerHTML = this.appData.message
                    },

                    fun1: function (json) {
                        this.retrospectives = json;
                    },

                    getRetrospectives: function () {
                        Model.RequestToServer('', Model.GetURL() + "/API/View", "", "", this.fun1)
                    },

                    addFeedback: function () {                                              

                        var data =
                        {
                            Name: document.getElementById("retrospectives").value, NameOfPerson: document.getElementById("nameOfPerson").value,
                            Body: document.getElementById("body").value, FeedbackType: document.getElementById("feedbackType").value
                        };
                        var json = JSON.stringify(data);

                        Model.RequestToServer(json, Model.GetURL() + "/API/AddFeedback", "", "", this.fun)
                    }
                }
            })
        }
        catch (err) {
            Model.Alert(err.message)
        }
    }
}
