// site.js

$(document).ready(function () {
    // 1. Smooth Scrolling for Navigation Links
    // Targets any element with the class 'smooth-scroll' and an href starting with '#'
    $('.smooth-scroll').on('click', function (e) {
        e.preventDefault();

        // Get the target ID from the data-target attribute
        // If data-target is missing, it falls back to lowercase button text
        var targetId = $(this).data('target') || $(this).text().trim().toLowerCase();
        if (targetId === 'get started' || targetId === 'start your project' || targetId === 'discuss your project') {
            targetId = 'contact';
        }
        if (targetId === '360 tours') {
            window.location = '/360tour';
        }
        if (targetId === 'tech-bricksit solutions') {
            window.location = '/home';
        }
        if (window.location.pathname === '/360tour') {
            window.location = '/home';
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

    $('#btn-contact').on('click', function (e) {
        e.preventDefault();

        // 1. Get the token from the hidden input field
        const token = $('input[name="__RequestVerificationToken"]').val();

        const formData = {
            Name: $('#name').val(),
            Email: $('#email').val(),
            Subject: $('#service').val(),
            Message: $('#message').val()
        };

        // 2. Validate basic length for the 'Message' (to match your C# MinLength)
        if (formData.Message.length < 10) {
            alert("The message must be at least 10 characters long.");
            return;
        }

        const $btn = $(this);
        $btn.prop('disabled', true).text('Sending...');

        // 3. Send the AJAX request
        $.ajax({
            url: '/home/contact',
            type: 'POST',
            data: formData,
            headers: {
                "RequestVerificationToken": token // Added security header
            },
            success: function (response) {
                alert("Message sent! We'll get back to you soon.");
                $('form')[0].reset();
            },
            error: function (xhr) {
                // If validation fails on the server (400), check responseJSON
                if (xhr.status === 400 && xhr.responseJSON) {
                    console.log("Validation errors:", xhr.responseJSON);
                    alert("Please check the form for errors.");
                } else {
                    alert("An error occurred. Status: " + xhr.status);
                }
            },
            complete: function () {
                $btn.prop('disabled', false).text('Send Message');
            }
        });
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

    // Hamburger menu logic
    $('#hamburger-btn').on('click', function () {
        $('#mobile-menu').show().removeClass('translate-x-full').addClass('translate-x-0');
    });
    $('#mobile-menu-close').on('click', function () {
        $('#mobile-menu').hide().removeClass('translate-x-0').addClass('translate-x-full');
    });
    // Optional: Hide menu when clicking outside
    $('#mobile-menu').on('click', function (e) {
        if (e.target === this) {
            $(this).hide().removeClass('translate-x-0').addClass('translate-x-full');
        }
    });
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