let index = 0;

function AddTag() {
    // Get a reference to the TagEntry input Element
    var tagEntry = document.getElementById("TagEntry");
    let tagList = document.getElementById("TagList");

    // Use search() function to detect an error state  | Validation
    let searchResult = search(tagEntry.value);
    if (searchResult != null) {
        // Trigger sweet alert for whatever condition is container in search results variable
        swalWithDarkButton.fire({
            html: `<span class='font-weight-bolder'>${searchResult.toUpperCase()}</span>`,
        });
    }
    else {
        // Create a new <select> option
        let newOption = new Option(tagEntry.value, tagEntry.value);
        tagList.options[index++] = newOption;
    }

    //clear out TagEntry control
    tagEntry.value = "";

    return true;
}

function DeleteTag() {
    let tagCount = 1;
    let tagList = document.getElementById("TagList");

    if (!tagList) return false;

    if (tagList.selectedIndex == -1) {
        swalWithDarkButton.fire({
            html: "<span class='font-weight-bolder'>CHOOSE A TAG BEFORE DELETING</span>"
        });
        return true;
    }

    while (tagCount > 0) {
        if (tagList.selectedIndex >= 0) {
            tagList.options[tagList.selectedIndex] = null;
            --tagCount;
        }
        else
            tagCount = 0;
        index--;
    }
}

$("form").on("submit", function () {
    $("#TagList option").prop("selected", "selected");
})

// Look for the tagValues variable to see if it has data, load tags
if (tagValues != '') {
    let tagArray = tagValues.split(",");
    for (let loop = 0; loop < tagArray.length; loop++) {
        // Load or replace options we have
        ReplaceTag(tagArray[loop], loop);
        index++;
    }
}

function ReplaceTag(tag, index) {
    let newOption = new Option(tag, tag)
    document.getElementById("TagList").options[index] = newOption;
}

function SelectBlog(blogId) {
    if (blogId == null) console.log("blogId is null");

    var blogList = document.getElementById("blogList");
    blogIndex = blogId - 1;
    blogList.selectedIndex = blogIndex;
}
SelectBlog(currentBlogId);

// The search() function will detect either an empty or a duplicate tag on this post
// & return an error string if an error is detected
function search(str) {
    if (str == "") {
        return 'Empty tags are not allowed';
    }

    var tagsEl = document.getElementById('TagList');
    if (tagsEl) {
        let options = tagsEl.options;
        for (let index = 0; index < options.length; index++) {
            if (options[index].value == str) {
                return `The tag #${str} was detected as a duplicate and is not allowed`;
            }
        }
    }
}

const swalWithDarkButton = Swal.mixin({
    customClass: {
        confirmButton: 'btn btn-danger btn-sm btn-block btn-outline-dark'
    },
    imageUrl: '/img/no-no-no-mutombo.gif',
    timer: 4000,
    buttonsStyling: false
});
