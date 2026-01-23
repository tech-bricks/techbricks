// site.js

$(document).ready(function () {
    // 1. Smooth Scrolling for Navigation Links
    // Targets any element with the class 'smooth-scroll' and an href starting with '#'
    $('.smooth-scroll').on('click', function (event) {
        if (this.hash !== "") {
            event.preventDefault(); // Prevent default anchor click behavior

            var hash = this.hash;

            // Use jQuery's animate() method for smooth page scroll
            $('html, body').animate({
                scrollTop: $(hash).offset().top - 70 // Adjust -70px for fixed header height
            }, 800, function () {
                // Optional: Add hash to URL once animation is complete
                // window.location.hash = hash; 
            });
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