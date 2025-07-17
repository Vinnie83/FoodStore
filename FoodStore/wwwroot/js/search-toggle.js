
document.addEventListener("DOMContentLoaded", function () {
    const toggle = document.getElementById("searchToggle");
    const form = document.getElementById("searchForm");

    if (toggle && form) {
        toggle.addEventListener("click", function (e) {
            e.preventDefault();
            const isVisible = form.style.display === "block";
            form.style.display = isVisible ? "none" : "block";
        });

        document.addEventListener("click", function (e) {
            if (!toggle.contains(e.target) && !form.contains(e.target)) {
                form.style.display = "none";
            }
        });
    }
});
