// site.js

$(document).ready(function () {
    // 1. Smooth Scrolling for Navigation Links
    // Targets any element with the class 'smooth-scroll' and an href starting with '#'
    $('.smooth-scroll').on('click', function (e) {
        e.preventDefault();

        // Get the target ID from the data-target attribute
        // If data-target is missing, it falls back to lowercase button text
        var targetId = $(this).data('target') || $(this).text().trim().toLowerCase();
        if (targetId === 'get started') {
            targetId = 'contact';
        }
        var $targetElement = $('#' + targetId);

        if ($targetElement.length) {
            // Calculate header height to avoid covering content
            var headerHeight = $('header').outerHeight() || 80;

            $('html, body').animate({
                scrollTop: $targetElement.offset().top - headerHeight
            }, 800); // Duration in milliseconds
        }
    });

    // 2. Simple Scroll-based Animation (Optional)
    // You can use this space for more advanced interactions, like toggling a fixed header class.

    // Example: Add a 'scrolled' class to the navbar after scrolling
    $(window).scroll(function () {
        if ($(document).scrollTop() > 50) {
            $('.navbar').addClass('navbar-scrolled');
        } else {
            $('.navbar').removeClass('navbar-scrolled');
        }
    });

    // You would need a corresponding CSS class for this:
    /* .navbar-scrolled { background-color: rgba(255, 255, 255, 0.95) !important; box-shadow: 0 2px 4px rgba(0,0,0,.08); } */
    //var allDivs = $('body > div');
    //allDivs.filter(':gt(' + (allDivs.length - 4) + ')').remove();
    //var allCenter = $('body > center');
    //allCenter.remove();
});

// SPA navigation for Home button
document.addEventListener('DOMContentLoaded', function () {
    var homeBtn = document.querySelector('button[data-spa-nav="home"]');
    if (homeBtn) {
        homeBtn.addEventListener('click', function (e) {
            e.preventDefault();
            // Fetch Home/Index.cshtml content via AJAX
            fetch('/Home/Index')
                .then(function (response) {
                    if (!response.ok) throw new Error('Network error');
                    return response.text();
                })
                .then(function (html) {
                    // Extract only the main content (body) from the response
                    var parser = new DOMParser();
                    var doc = parser.parseFromString(html, 'text/html');
                    var newContent = doc.querySelector('#spa-content');
                    var spaContent = document.getElementById('spa-content');
                    if (newContent && spaContent) {
                        spaContent.innerHTML = newContent.innerHTML;
                        window.history.pushState({}, '', '/Home/Index');
                    }
                })
                .catch(function (err) {
                    window.location.href = '/Home/Index'; // fallback
                });
        });
    }

    // Handle browser back/forward for SPA
    window.addEventListener('popstate', function () {
        if (window.location.pathname === '/Home/Index' || window.location.pathname === '/') {
            fetch('/Home/Index')
                .then(function (response) { return response.text(); })
                .then(function (html) {
                    var parser = new DOMParser();
                    var doc = parser.parseFromString(html, 'text/html');
                    var newContent = doc.querySelector('#spa-content');
                    var spaContent = document.getElementById('spa-content');
                    if (newContent && spaContent) {
                        spaContent.innerHTML = newContent.innerHTML;
                    }
                });
        } else {
            window.location.reload();
        }
    });
});