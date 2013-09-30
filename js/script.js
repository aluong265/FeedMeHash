$(document).ready(function () {
    // Place watermark on the form fields so that the user knows what to enter in the field
    var $tbxHashtag = $("[id$='tbxHashtag']");
    $tbxHashtag.watermark('Enter a hashtag');

    var $tbxFilter = $("[id$='tbxFilter']");
    $tbxFilter.watermark('Enter a hashtag you want to filter out');

    // Validate form fields before submission
    $("form").submit(function () {
        var $findTweets = $("[id$='btnFindTweets']");

        // Validate empty field for Hashtag and invalid input for Hashtag and Filter
        var failedValidation = false;
        if (IsEmptyField($tbxHashtag, "Please enter a hashtag to find tweets", "hashtag-error")) {
            failedValidation = true;
        }
        else if (IsValidInput($tbxHashtag, "The hashtag you entered is invalid. Please enter letters and numbers only", "hashtag-error")) {
            failedValidation = true;
        }
        // Only validate the filter if a value has been enter in the textfield
        if ($tbxFilter.val() != "" && $tbxFilter.val() != undefined) {
            if (IsValidInput($tbxFilter, "The hashtag you entered is invalid. Please enter letters and numbers only", "filter-error", true)) {
                failedValidation = true;
            }
        }
        else {
            // Remove error message if it's empty
            $(".filter-error").text("");
        }

        // If any validation failed, do not submit post
        if (failedValidation) {
            return false;
        }
    });

    // Sort results using JQuery
    $tweetContainer = $('.tweet-container');
    $tweetResultsContainer = $('.tweet-results-container');
    $("select").change(function () {
        var sortBy = $(this).find(":selected").val();
        if (sortBy == "aDate") {
            $tweetContainer.sort(sortAscending).appendTo($tweetResultsContainer);
        }
        else if (sortBy == "dDate") {
            $tweetContainer.sort(sortDescending).appendTo($tweetResultsContainer);
        }
    });
});

// Sort from oldest date to recent
function sortDescending(a, b) {
    var date1 = $(a).find(".date").text();
    date1 = new Date(date1);
    var date2 = $(b).find(".date").text();
    date2 = new Date(date2);

    return date1 - date2;
};

// Sort from recent date to oldest
function sortAscending(a, b) {
    var date1 = $(a).find(".date").text();
    date1 = new Date(date1);
    var date2 = $(b).find(".date").text();
    date2 = new Date(date2);

    return date2 - date1;
};

// Validate the field is empty
function IsEmptyField($input, msg, cssclass) {
    var $findTweets = $("[id$='btnFindTweets']");
    var field = $input.val();

    // If field is empty or undefined
    // Display error message
    if (field == "" || field == undefined) {
        // Check to see if error message is already displayed
        if ($("." + cssclass).length == 0) {
            if ($(".hash-feed-container-results").length == 0) {
                // Handles on page load
                $input.after('<span class="' + cssclass + ' error">' + msg + '</span>');
            }
            else {
                // Handles on post back
                $findTweets.after('<span class="' + cssclass + ' error">' + msg + '</span>');
            }
        }
        else {
            $("." + cssclass).text(msg);
        }
        return true;
    }
    return false;
}

// Validate the input has letters and numbers
function IsValidInput($input, msg, cssclass, isFilter) {
    var $findTweets = $("[id$='btnFindTweets']");
    var field = $input.val();

    // Check if input is valid, if not, display error message
    // Input has to be letters and numbers only
    if (field.match(/^[^A-Z0-9]*$/i)) {
        // Check to see if error message is already displayed
        if ($("." + cssclass).length == 0) {
            if ($(".hash-feed-container-results").length == 0) {
                // Handles on page load
                $input.after('<span class="' + cssclass + ' error">' + msg + '</span>');
            }
            else if ($(".hash-feed-container-results").length != 0 && !isFilter) {
                // Handles on post back
                $findTweets.after('<span class="'+ cssclass + ' error">' + msg + '</span>');
            }
            else if ($(".hash-feed-container-results").length != 0 && isFilter) {
                $(".sort-container").after('<span class="' + cssclass + ' error">' + msg + '</span>');
            }
            else {
                $("." + cssclass).text(msg);
            }
        }
        else {
            $("." + cssclass).text(msg);
        }
        return true;
    }
    return false;
}