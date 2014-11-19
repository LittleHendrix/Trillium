(function($) {

    $(document).foundation();

    initTypeAhead();

    function initTypeAhead() {
        var results = {};
        $('.typeahead').typeahead({
            minLength: 3,
            items: 20,
            source: function (query, process) {
                $.post('/search?json=true&searchTerms=' + query, { q: query, limit: 8 }, function (data) {
                    titles = [];
                    results = {};
                    $.each(data, function (i, result) {
                        results[result.Title] = result;
                        titles.push(result.Title);
                    });
                    process(titles);
                });
            },
            matcher: function (item) { return true; },
            updater: function (item) {
                var selectedValue = '';
                if (item == undefined) {
                    selectedValue = $('.typeahead').val();
                    $('.typeahead').closest('form').submit();
                }
                else {
                    selectedValue = item;
                    window.location = results[item].Url;
                }
                return $('.typeahead').val();
            }
        });
    }

    $('a[href*=#]:not([href=#])').click(function () {
        if (location.pathname.replace(/^\//, '') == this.pathname.replace(/^\//, '') && location.hostname == this.hostname) {
            var target = $(this.hash);
            target = target.length ? target : $('[name=' + this.hash.slice(1) + ']');
            if (target.length) {
                $('html,body').animate({
                    scrollTop: target.offset().top
                }, 1000);
                return false;
            }
        }
    });

    // custom attr client validation
    $.validator.addMethod('checkPot', function (value, element) {
        return this.optional(element) || !value;
    }, '');

    $.validator.unobtrusive.adapters.add('honeypot', {}, function (options) {
        options.rules['checkPot'] = true;
        options.messages['checkPot'] = options.message;
    });

})(jQuery);