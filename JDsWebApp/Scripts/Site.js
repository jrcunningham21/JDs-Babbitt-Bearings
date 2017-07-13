//marks a step as completed in the viewmodel
//The page calling this must have a field such as "@Html.HiddenFor(m => m.IsCompleted, new { id = "IsCompleted" })"
function markStepAsCompleted() {
    $("#IsCompleted").val(true);
}
// Adds the specified message to the specified job's Change Log Entry
function addJobChangeLogEntry(jobId, message) {
    var d = new Date().toLocaleString();

    var strMethodUrl = '/JobHistory/AddJobChangeLogEntry?dateStr=' + d + '&jobId=' + jobId + '&message=' + message;

    $.getJSON(strMethodUrl, function (retVal) {
        // Don't have to do anything
        //alert(retVal);
    });
}

// Shows the part detail popup for the given partId. The page you call this from should have rendered PartInfoPopUpModal on it
function showPartPopup(partId) {
    showPartPopup(partId, 0);
};

//formats phone number elements for display as (999) 999-9999
function formatPhoneNumber(phonenumber, label) {
    if (phonenumber != null && phonenumber != "") {
        var formattedNumber = '(' + phonenumber.substr(0, 3) + ') ' + phonenumber.substr(3, 3) + '-' + phonenumber.substr(6, 4);
        return formattedNumber + " " + label;
    }
    else
        return "";
};

//turns off autofill and autocomplete on text input elements
function turnOffAutofill(formid) {
    $(formid).find($("input[type='text']")).each(function (index, value) {
        $(this).attr('autocomplete', 'off');
        $(this).attr('autofill', 'off');
        $(this).attr('onkeydown', 'stopAutofillDropdown(event)');
    });
};

//masks phone number input elements
function mask(phonenumberid) {
    $(phonenumberid).mask("(999) 999-9999");
}


//stops autofill dropdowns from occuring on down arrow (chrome only)
function stopAutofillDropdown(event) {
    if (event.keyCode == "40") {    //downArrow keyCode == 40
        event.preventDefault();
    }
}

// Shows the part detail popup for the given partId. The page you call this from should have rendered PartInfoPopUpModal on it
// Opens to the specific index
function showPartPopup(partId, tabIndex) {
    var myUrl = '/Part/PartInfoPopUp?id=' + partId + '&tabIndex=' + tabIndex;
    $.ajax({
        url: myUrl,
        type: 'GET',
        data: { partID: partId },
        success: function (data) {
            $('#partInfoContentDiv').html(data);

            //If the calling page has a PartNotes editor textbox, append its value (if any) to the Part Notes in the pop up
            if ($("#BB1_PartNotes").length != 0) {
                var modelNotes = $("#PartInfoPopUp_PartNotes").text();
                var BB1_Notes = $("#BB1_PartNotes").val();
                $("#PartInfoPopUp_PartNotes").text(modelNotes + "\r\n" + BB1_Notes);
            }
            $('#partInfoContentDiv').fadeIn('fast');
            $('#partInfoPopupModal').modal('show');
        },
        error: function (e) {
            bootbox.alert("Error getting part pop-up", function () { });
            $("#partInfoPopupModal").modal('hide');
        }
    });

};

function hasErrors(container) {
    var anyError = false;
    $(container).find("input").each(function () {
        if ($(this).attr("data-val-required") &&
            $(this).data("val-required").toLowerCase().indexOf("required") >= 0 &&
            $(this).val() == "") {
            anyError = true;
            $(this).addClass("error");
        }
        else if ($(this).hasClass("error")) {
            $(this).removeClass("error");
        }
    });

    return anyError;
}



function setCurrentStep(sd, currentStepAsideId) {

    $('#workflowContainer').children().each(function (i) {

        var thisId = $('#workflowContainer').children()[i].id;
        var thisElem = $('#' + thisId);

        if (currentStepAsideId === thisId) {
            thisElem.addClass('aside-entry-active');
        } else if (sd[i]) {
            thisElem.addClass('aside-entry-complete');
        } else {
            thisElem.addClass('aside-entry-incomplete');
        }

    });
}

function CheckPartForFiles(partId) {
    // Checks the given part to see if both files and photos have been saved for this part. 
    // Returns true if both, false if not both
    var myUrl = '/Part/CheckPartForFiles?partId=' + partId;
    return $.ajax({
        url: myUrl,
        type: 'GET',
        data: { partID: partId },
        error: function (e) {
            bootbox.alert("Error checking part for files", function () { });
        }
    });
}

function CheckForMinimumCustomerSizes(partId, callbackFunc) {
    // Checks the given part to see if a minimum customer size amount of data has been set
    // Specifically, you need a Bore diameter and one OD measurement
    // Returns true if both, false if not both
    var myUrl = '/Part/CheckPartForCustomerSizes?partId=' + partId;
    $.ajax({
        url: myUrl,
        type: 'GET',
        data: { partID: partId },
        success: function (data) {

            callbackFunc(data);
        },
        error: function (e) {
            bootbox.alert("Error checking part for files", function () { });
        }
    });
}

function flagForCustomerApproval(message, pid) {
    kendo.ui.progress($('body'), true);
    var myUrl = '/Part/FlagPartForCustomerApproval';
    var values = {
        "partId": pid,
        "message": message
    };

    $.ajax({
        url: myUrl,
        type: 'POST',
        data: values,
        success: function (data) {
            window.location.href = "/";
        },
        error: function (e) {
            bootbox.alert("Error", function () { });
        }
    });
}


function onShow(e) {
    if (!$("." + e.sender._guid)[1]) {
        var element = e.element.parent(),
            eWidth = element.width(),
            eHeight = element.height(),
            wWidth = $(window).width(),
            wHeight = $(window).height(),
            newTop, newLeft;

        newLeft = Math.floor(wWidth / 2 - eWidth / 2);
        newTop = Math.floor(wHeight / 2 - eHeight / 2);

        e.element.parent().css({ left: newLeft });
    }
}

function toggleValidationMessage(id, message) {
    message = message || "";
    var target = $("#" + id);

    if (target.length) {
        if (target.hasClass("field-validation-valid") === true && message !== "") {
            target.removeClass("field-validation-valid");
            target.addClass("field-validation-error");
            target.html(message);
        } else if (target.hasClass("field-validation-error") === true && message === "") {
            target.addClass("field-validation-valid");
            target.removeClass("field-validation-error");
            target.html("");
        } else if (target.hasClass("field-validation-error") === true && message !== "") {
            target.html(message);
        }
    }
}

//Params
//selector: Takes a JQuery Selector which should point to a select that it needs to add the add event to
//id: takes the controlID for the type of selector it is
//Purpose: Add a add event to the add button on the BootStrap MultiSelect to add an option to the select element then rebuild multiselect
//to add it to the list of items
function BindMultiSelectAddButton(selector, id) {
    selector.next().find("i.glyphicon-search").on("click", function () {
        var addValue = selector.next().find("input.multiselect-search").val();

        var numberOfMatchingValues = selector.children().filter(function () {
            return $(this).text() === addValue;
        }).length;

        if (addValue !== "" && addValue !== null && numberOfMatchingValues == 0) {
            $.ajax({
                type: 'POST',
                url: '/EmployeePortal/AutoComplete/AddNewPart/',
                data: {
                    id: id,
                    value: addValue
                },
                success: function (data) {
                    selector.append("<option value='" + data.AutoCompleteId + "'>" + addValue + "</option>");
                    if (selector.children().length > 0) {
                        selector.children("option[value='none']").first().remove();
                    }
                    selector.multiselect("rebuild");
                    selector.multiselect("select", [data.AutoCompleteId]);

                    BindMultiSelectAddButton(selector, id);
                },
                error: function (data) {
                    bootbox.alert("Error in AJAX AddNewPart", function () { });
                }
            });
        } else if (numberOfMatchingValues > 0) {
            notification.show({ message: "Value already exists " }, "successful");
        } else if (addValue !== "" || addValue !== null) {
            notification.show({ message: "Please enter a value to add " }, "successful");
        }
    });
}

//Params
//e: The touch event
//controlID: The controlID of the select
//Purpose: Takes the autocompleteitem and deletes it from the control by making an ajax call to remove the part then
//removing the option from the multiselect then rebuilding the multiselect so it won't show up.
function DeleteLineItem(e, controlID) {
    e.preventDefault();
    var context = $(e.currentTarget);
    var autocompleteID = context.prev().val();

    $.ajax({
        type: 'POST',
        url: '/EmployeePortal/AutoComplete/DeletePart/' + autocompleteID,
        success: function (data) {
            var selector = $("div.btn-group").has(context).prev();
            selector.children("option[value='" + autocompleteID + "']").first().remove();
            if (selector.children().length === 0) {
                selector.append("<option value='none' disabled>None</option>");
            }
            selector.multiselect("rebuild");
            BindMultiSelectAddButton(selector, controlID);
        },
        error: function (data) {
            bootbox.alert("Error in AJAX DeletePart", function () { });
        }
    });
}


//Params
//array: the array to convert
//context: the jquery selector for the multiselect to go though
//Purpose: Converts each value in the array from its text value to its actually value
function ConvertTextArrayToValueArray(array, context) {
    if (array != null) {
        array.forEach(function(element, index, array) {
            array[index] = context.children().filter(function() {
                return $(this).text() == element;
            }).first().val();
        });
    } else {
        array = [];
    }

    return array;
}

//Params
//array: the array to convert
//context: the jquery selector for the multiselect to go though
//Purpose: Converts each value in the array from its value to its text
function ConvertValueArrayToTextArray(array, context) {
    if (array != null || array != undefined) {
        array.forEach(function(element, index, array) {
            array[index] = context.children().filter(function() {
                return $(this).val() == element;
            }).first().text();
        });
    } else {
        array = [];
    }

    return array;
}

// This global variable is to represent a function call callback that the signoff
// can call when done validating a PIN. If it gets called, the PIN was valid.
// This is for the 13 steps of the Babbitt bearing workflow only.
var workflowSignoffCallback = null;

////////////////////////////////////////////////////////////////////////////////
// Function : printf(f, [item1[, [item2[, ...]]])
// Author   : Hector J. Rivas
// Purpose	: act like C/C++ printf (insert arguments into placeholders)
////////////////////////////////////////////////////////////////////////////////
function printf(f) {
    for (var i = 1, r = f; i < arguments.length; i++)
        r = r.replace(new RegExp('\\{' + (i - 1) + '\\}', 'g'), arguments[i]);

    return r;
}

////////////////////////////////////////////////////////////////////////////////
// Function : captureCompletedSignoffs
// Author   : Hector J. Rivas
// Purpose  : capture successfully completed signoffs When a signoff modal is
//            cancelled
// Notes    : Relies on several assumptions about signoff controls naming,
//            from the modal to the rest of the modal and main page controls.
////////////////////////////////////////////////////////////////////////////////
function captureCompletedSignoffs() {
    // assuming there is a modal named 'signOffGroupModal' with signoffText
    // classed inputs
    $("#signOffGroupModal input.signoffText").each(function () {
        // the input is signed
        if (this.value) {
            // find the main page equivalent signoff, e.g., if this
            // has id = "BB1_FinalInspectionApprovedBy1", we're looking for
            // "BB1_FinalInspectionApprovedBy"
            var baseId = this.id.substring(0, this.id.length - 1),
                signoffInput = $('#' + baseId),
                signoffButton = $('#' + baseId + "SO"),
                signoffIcon = signoffButton.children(".glyphicon:first");

            // if the main page signoff is found and empty (not signed off)
            if (!signoffInput.val()) {
                // give it a value and the looks
                signoffInput.val(this.value);

                signoffButton.removeClass("btn-danger");
                signoffIcon.removeClass("glyphicon-minus").addClass("glyphicon-ok");
            }
        }
    });
}

function AskForSaveBeforeLinkRedirect(saveCallBack, link) {
    bootbox.dialog({
        message: "Do you want to save before leaving the page",
        title: "Confirm Save",
        buttons: {
            yes: {
                label: "Yes",
                className: "btn-primary",
                callback: function() {
                    saveCallBack(true);
                    window.location.href = link;
                }
            },
            no: {
                label: "No",
                className: "btn-danger",
                callback: function () {
                    window.location.href = link;
                }
            },
            cancel: {
                label: "Cancel",
                className: "btn-success",
                callback: function () {

                }
            }
        }
    });
}

// This function is used to format the numbers 
// being injected into grids to have a minimum
// decimal place
function formatToMinDecimals(measurement, precision) {
    var split;

    if (measurement.indexOf('.') >= 0) {
        split = measurement.split('.');
        if (split[1].length < precision) {
            var diff = precision - split[1].length

            for (var i = 0; i < diff; i++) {
                measurement += '0';
            }
        }
    } else if (measurement.length == 0) {
        measurement = '';
    } else {
        measurement += '.';
        for (var i = 0; i < precision; i++) {
            measurement += '0';
        }
    }

    return measurement;
}

/**************************Take Photo stuff****************************/
//Provides functionality for taking photos for via camera input.

/**Photo params**/
var width = 800;    // We will scale the photo width to this
var height = 0;     // This will be computed based on the input stream
var streaming = false;


var video;
var canvas;
//var videoSource;
var videoSelect;
var audioSelect;
var windowID;

/**Initializes the modeless, maximized window that shows the camera stream**/
function initTakePhotoWindow(windowElementID, imgElementID, partID) {
    var windowOptions = {
        width: "500px",
        title: "Take Photo",
        visible: false,
    };

    $(windowElementID).kendoWindow(windowOptions);
    $(windowElementID).data("kendoWindow").open();
    $(windowElementID).data("kendoWindow").maximize();
    $(windowElementID).parent().css("z-index", "1050");

    videoSelect = $(windowElementID).find("#videoSource").get(0);
    video = $(windowElementID).find("#video").get(0);
    canvas = $(windowElementID).find("#canvas").get(0);

    startbutton = $(windowElementID).find("#photoShutterBtn").get(0);
    $(startbutton).off();

    $(startbutton).on("click", function (event) {
        $("#savePhotoBtn").attr('disabled', 'disabled');
        $("#btnupload").attr('disabled', 'disabled');
        $("#takePhotoBtn").attr('disabled', 'disabled');
        
        if (takePhoto(imgElementID, partID)) {
            event.preventDefault();
            emptySelection($(windowElementID).find("#videoSource"));
            cleanup();
            $(windowElementID).data("kendoWindow").close();
        }
        else {
            connectCameraPrompt();
        }

    });

    cancelbutton = $(windowElementID).find("#cancelPhotoShutterBtn").get(0);
    $(cancelbutton).off();
    $(cancelbutton).on("click", function(event) {
       emptySelection($(windowElementID).find("#videoSource"));
        cleanup();
        $(windowElementID).data("kendoWindow").close(); 
    });

    // MediaStreamTrack is deprecated... consider implementing MediaDevices.enumerateDevices() when milestone 3 begins (AcE 2/16/2016)
    MediaStreamTrack.getSources(gotMediaSources);   //find and put camera inputs in a selectable box
    

    //add the camera toggle button every time the window is instantiated, so we don't reappend camera options
    var toggleBtn = document.createElement("BUTTON");
    toggleBtn.setAttribute('id', 'cameraToggleBtn');
    toggleBtn.setAttribute('class', 'btn_overlay');
    $(windowElementID).find('#camera-area').get(0).appendChild(toggleBtn);
    
    cameraTogglebtn = $(windowElementID).find("#cameraToggleBtn").get(0);
    $(cameraTogglebtn).off();
    $(cameraTogglebtn).on("click", function(event) {
        console.log(windowElementID);
        var currentSelection = $(windowElementID).find("#videoSource").get(0).selectedIndex;
        if (videoSelect.length != 0 ){  
            if (currentSelection == videoSelect.length - 1)
                currentSelection = 0;
            else
                currentSelection++;
            console.log(currentSelection);
            $(windowElementID).find("#videoSource > option").eq(currentSelection).attr('selected', 'selected');
            $(windowElementID).find("#videoSource").trigger("onchange");
        }
    });
    
    startStream(windowElementID, imgElementID, partID);
    clearPhoto();
}

/**Starts the video stream for the window that called it **/
function startStream(windowElementID, imgElementID, partID) {
    //if we're already streaming, stahp
    if (window.stream) {
        video.src = null;
        window.stream.getTracks()[0].stop();
    }

    var videoSource = videoSelect.value;

    navigator.getMedia = (navigator.getUserMedia ||
                           navigator.webkitGetUserMedia ||
                           navigator.mozGetUserMedia ||
                           navigator.msGetUserMedia);

    navigator.getMedia(
        {
            video: {
                optional: [{ sourceId: videoSource }]
            },
            audio: false
        },
        function (stream) {
            if (navigator.mozGetUserMedia) {
                video.mozSrcObject = stream;
            } else {
                var vendorURL = window.URL || window.webkitURL;
                video.src = vendorURL.createObjectURL(stream);
            }
            //localStream = stream;
            window.stream = stream;
            video.play();
        },
        function (err) {
            //console.log("An error occured! " + err);

            connectCameraPrompt();
        }
        );
    video.addEventListener('canplay', function (ev) {
        if (!streaming) {
            height = video.videoHeight / (video.videoWidth / width);
            // Firefox currently has a bug where the height can't be read from
            // the video, so we will make assumptions if this happens.

            if (isNaN(height)) {
                height = width / (4 / 3);
            }

            video.setAttribute('width', width);
            video.setAttribute('height', height);
            canvas.setAttribute('width', width);
            canvas.setAttribute('height', height);
            streaming = true;
        }
    }, false);
    videoSelect.onchange = startStream;
}

function connectCameraPrompt() {
    setTimeout(function () {
        bootbox.confirm("Please connect a camera", function () {
        });
    }, 500);
}

/**Captures a photo, converts it to a .png data url, and draws a preview onto the canvas**/
function takePhoto(imgElementID, partID) {
    var tookPhoto = false;

    if (streaming) {
        canvas.width = width;
        canvas.height = height;

        var context = canvas.getContext('2d');
        context.drawImage(video, 0, 0, width, height);

        var imgContents = canvas.toDataURL('image/png');
        $(imgElementID).attr("src", imgContents);

        //Right now the entire contents of the photo is set in the URL - let's upload the file, save it,
        //and re-jigger the URL to keep the database records light
        imgContents = imgContents.replace('data:image/png;base64,', '');
        var values =
        {
            "partId": partID,
            "data": imgContents
        }

        var actionstring = "/Part/SaveDataURLtoFile/";
        var urlString = "";

        kendo.ui.progress($("#savePhotoBtn"), true);

        $.post(actionstring, values, function (data, status, xhr) {
            $(imgElementID).attr("src", data);
            var isFile = imgElementID.search("file");
            if (isFile !== -1) {
                $("#fileUrl").val(data);
            }
            $("#savePhotoBtn").removeAttr('disabled');
            $("#btnupload").removeAttr('disabled');
            $("#takePhotoBtn").removeAttr('disabled');
            kendo.ui.progress($("#savePhotoBtn"), false);
        });

        tookPhoto = true;
    }
    else {
        $("#btnupload").removeAttr('disabled');
        $("#takePhotoBtn").removeAttr('disabled');
    }
    
    return tookPhoto;
}

/**Clears the photo element and dataURL to gray**/
function clearPhoto(imgElementID) {
    var context = canvas.getContext('2d');
    context.fillStyle = "#AAA";
    context.fillRect(0, 0, canvas.width, canvas.height);

    var data = canvas.toDataURL('image/png');
    $(imgElementID).attr("src", data);
}

/**find and append camera sources**/
function gotMediaSources(sourceInfos) {
    
    for (var i = 0; i < sourceInfos.length; i++) {
        var sourceInfo = sourceInfos[i];

        var option = document.createElement('option');
        
        option.value = sourceInfos[i].id;
        if (sourceInfo.kind === 'video') {
            option.text = sourceInfo.label || 'camera' + (videoSelect.length + 1);
            videoSelect.appendChild(option);
        }
    }
}

/**to empty a select element (via JQuery selector) for reuse/repopulation later**/
function emptySelection(selectElement) {
    selectElement.empty();
}

//cleanup elements that should be reappended on new modal instances
function cleanup() {
    var toggleBtn = document.getElementById("cameraToggleBtn");
    toggleBtn.parentNode.removeChild(toggleBtn);
}
/*********************end Take Photo stuff*****************************/

//general backspace disable backnav
$(document).on("keydown", function (e) {
    if (e.which === 8){
        if (document.activeElement.tagName != "INPUT" && document.activeElement.tagName != "TEXTAREA") 
            e.preventDefault();
            
        //if inputs/textareas are readonly, backspace defaults to back-nav, so prevent that too
        else if ($("#" + document.activeElement.id).is('[readonly="true"]')) 
            e.preventDefault(); 
    }
});


