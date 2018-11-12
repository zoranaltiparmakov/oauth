const API = "http://localhost:64905/api/";
const CLIENT_ID = "test123";
const CLIENT_SECRET = "test123";

// define global function for removing all attr from element
$.fn.removeAttributes = function () {
    return this.each(function () {
        var attributes = $.map(this.attributes, function (item) {
            return item.name;
        });
        var img = $(this);
        $.each(attributes, function (i, item) {
            img.removeAttr(item);
        });
    });
}

function isExpired(tokenCredentials) {
    credentials = JSON.parse(tokenCredentials);
    expire_date = new Date(credentials[".expires"]).getTime();
    dateNow = Date.now();
    if (expire_date < dateNow) {
        return true;
    }
}

// on window load
$(function () {
    // check if LocalStorage token is expired and remove it
    if (localStorage.getItem("credentials") !== null && isExpired(localStorage.getItem("credentials"))) {
        localStorage.removeItem("credentials");
    }
    // check if user is logged in and say hello
    if (localStorage.getItem("credentials") !== null) {
        // some interesting effect..
        $('#nav-item-login a').fadeOut(1000, function () {
            $(this).text("");
            $(this).removeAttributes();
            $(this).addClass("hello-username");
        }).show(200, function () {
            credentials = JSON.parse(localStorage.getItem("credentials"));
            $(this).text("Hello " + credentials.username + "!");
        });

        // get all tasks and fill table
        fetch(API + "Task", {
            headers: {
                'Authorization': 'Bearer ' + JSON.parse(localStorage.getItem("credentials")).access_token
            }
        }).then(data => data.json())
            .then(data => {
                for (task of data) {
                    manage_trash = "<a href='#' onclick='delete_task(this);'><i class='fa fa-trash' id='task_delete_" + task.TaskID + "'></i></a>";
                    row = "<tr><td>" + task.TaskID + "</td><td>" + task.Name + "</td><td>" +
                        new Date(task.Created_On).toDateString() + "</td><td>" +
                        task.UserID + "</td><td>" + manage_trash + "</td></tr>";
                    $("#tasks_list tbody").append(row);
                }
            });
    }
});

// create new task
$('#submit_task').click(function () {
    task_name = $('#task_name')[0].value;
    data = { Name: task_name };

    fetch(API + "Task", {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + JSON.parse(localStorage.getItem("credentials")).access_token
        },
        // stringify for transport
        body: JSON.stringify(data)
    }).catch(err => {
    }).then(result => {
            location.reload();
        });

});

$('#login-submit').on('click', function () {
    errors = false;
    if (!$('#username-login')[0].value.length || !$('#password-login')[0].value.length) {
        $('#login-errors').append("<li>You must enter credentials!</li>");
        errors = true;
    }
    $("login-errors").fadeIn(15).delay(3500).fadeOut(1000);
    // send form if there are not errors
    if (!errors) {
        //TODO - Security: send client and secret base64 encoded in Authorization Basic HTTP Header
        // or do not send at all
        data = "grant_type=password&username=" + $("#username-login").val() + "&password=" +
            $("#password-login").val() + "&client_id=" + CLIENT_ID + "&client_secret=" + CLIENT_SECRET + "&scope=read, write";

        fetch(API + 'token', {
            method: 'POST',
            headers: {
                // send as urlencoded to get rid of pre-flight OPTIONS method (CORS)
                'Content-Type': 'application/x-www-form-urlencoded',
            },
            body: data
        }).then(token => token.json())
            .then(credentials => {
                localStorage.setItem("credentials", JSON.stringify(credentials));
            })
            .then(hideLogin => {
                location.reload();
            });
    }
});

/* CRUD methods on tasks */
// When trash is clicked, get id of the post, and send DELETE method to the server
function delete_task(element) {
    id = $(element)[0].childNodes[0].attributes["id"].value;
    idNum = id.match(/\d+/).map(Number)[0];

    fetch(API + "Task/" + idNum, {
        method: 'DELETE',
        headers: {
            'Authorization': 'Bearer ' + JSON.parse(localStorage.getItem("credentials")).access_token
        }
    }).then(response => {
            $(element).parents("tr")[0].remove();
        });
}

// When pencil icon is clicked, get id of the task, make task name field editable, save, and send via PUT
function edit_task(element) {
    id = $(element)[0].childNodes[0].attributes["id"].value;
    text = $(element).parents("tr").find("td:eq(1)")[0];
}

$('#deleteTasks').on('click', '.btn-ok', function (e) {
    fetch(API + "Task", {
        method: 'DELETE',
        headers: {
            'Authorization': 'Bearer ' + JSON.parse(localStorage.getItem("credentials")).access_token
        }
    }).then(response => {
        $("#tasks_list tbody").empty();
    });
});

function download(data, filename, type) {
    var file = new Blob([data], {type: type});
    if (window.navigator.msSaveOrOpenBlob) // IE10+
        window.navigator.msSaveOrOpenBlob(file, filename);
    else { // Others
        var a = document.createElement("a"),
                url = URL.createObjectURL(file);
        a.href = url;
        a.download = filename;
        document.body.appendChild(a);
        a.click();
        setTimeout(function() {
            document.body.removeChild(a);
            window.URL.revokeObjectURL(url);  
        }, 0); 
    }
}

$('#export_tasks').on('click', function (e) {
    fetch(API + "Task", {
        headers: {
            'Authorization': 'Bearer ' + JSON.parse(localStorage.getItem("credentials")).access_token
        }
    }).then(data => data.json())
        .then(data => {
            download(JSON.stringify(data), "data.json", "json");
        });
});
