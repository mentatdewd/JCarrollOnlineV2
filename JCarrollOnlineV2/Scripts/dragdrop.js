document.addEventListener("DOMContentLoaded", function () {
    const textarea = document.getElementById("Content");

    if (!textarea) {
        console.warn("DragDrop: #Content not found on page.");
        return;
    }

    ["dragenter", "dragover", "dragleave", "drop"].forEach(eventName => {
        textarea.addEventListener(eventName, e => {
            e.preventDefault();
            e.stopPropagation();
        });
    });

    textarea.addEventListener("dragover", function () {
        textarea.classList.add("drag-hover");
    });

    textarea.addEventListener("dragleave", function () {
        textarea.classList.remove("drag-hover");
    });

    textarea.addEventListener("drop", function (e) {
        textarea.classList.remove("drag-hover");

        const file = e.dataTransfer.files[0];
        if (!file) return;

        const formData = new FormData();
        formData.append("file", file);

        fetch("/Uploads/UploadToB2", {
            method: "POST",
            body: formData
        })
            .then(r => r.json())
            .then(data => {
                if (data.url) {
                    insertAtCursor(textarea, "\n\n" + data.url + "\n\n");
                    textarea.dispatchEvent(new Event("input"));
                } else {
                    alert("Upload failed");
                }
            })
            .catch(err => console.error(err));
    });

    function insertAtCursor(field, text) {
        const start = field.selectionStart;
        const end = field.selectionEnd;
        const before = field.value.substring(0, start);
        const after = field.value.substring(end);

        field.value = before + text + after;
        field.selectionStart = field.selectionEnd = start + text.length;
        field.focus();
    }
});
