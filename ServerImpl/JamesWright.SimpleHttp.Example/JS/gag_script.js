function submit_gagq() {
    var data = JSON.stringify({
        "subject": document.getElementById("subject").value,
        "topic": document.getElementById("topic").value
    });
    var settings = {
        "async": true,
        "crossDomain": true,
        "url": "http://localhost:8005/gagq",
        "method": "POST",
        "xhrFields": {
            "withCredentials": true
        },
        "headers": {
            "content-type": "application/x-www-form-urlencoded",
            "cache-control": "no-cache",
            "postman-token": "87dfacbe-364d-57df-9f22-9bd4752d7b0c"
        },
        "data": data
    }
    $.ajax(settings).done(function (response, status, jqXHR) {
        console.log(response);
        var myHeader = jqXHR.getResponseHeader("answer_status");
        if (myHeader.valueOf() === new String('success').valueOf()) {
            window.location.href = "http://localhost:8005/question";
        }
        else {
            var message = jqXHR.getResponseHeader("error_message");
            window.alert(message);
        }

    });
}


function submit_gagt() {
    var policy = document.getElementsByName("answerPolicy");
    for (var i = 0; i < norms.length; i++) {
        if (policy[i].checked == true) {
            selectedPolicy = policy[i].value;
        }
    }

    var data = JSON.stringify({
        "subject": document.getElementById("subject").value,
        "topic": document.getElementById("topic").value,
        "number": document.getElementById("numOfQuestions").value,
        "answerPolicy": policy
    });
    var settings = {
        "async": true,
        "crossDomain": true,
        "url": "http://localhost:8005/gagt",
        "method": "POST",
        "xhrFields": {
            "withCredentials": true
        },
        "headers": {
            "content-type": "application/x-www-form-urlencoded",
            "cache-control": "no-cache",
            "postman-token": "87dfacbe-364d-57df-9f22-9bd4752d7b0c"
        },
        "data": data
    }
    $.ajax(settings).done(function (response, status, jqXHR) {
        console.log(response);
        var myHeader = jqXHR.getResponseHeader("answer_status");
        if (myHeader.valueOf() === new String('success').valueOf()) {
            //window.alert(response);
            window.location.href = "http://localhost:8005/main";
        }
        else {
            var message = jqXHR.getResponseHeader("error_message");
            window.alert(message);
        }

    });
}