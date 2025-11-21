// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

(function ($) {
    $(function () {
        var $hours = $("#HoursWorked");
        var $rate = $("#HourlyRate");
        var $total = $("#CalculatedTotal");

        if (!$hours.length || !$rate.length || !$total.length) {
            return;
        }

        function parseDecimal(value) {
            var normalized = (value || "").toString().replace(",", ".");
            var parsed = parseFloat(normalized);
            return isNaN(parsed) ? null : parsed;
        }

        function updateTotal() {
            var hours = parseDecimal($hours.val());
            var rate = parseDecimal($rate.val());

            if (hours !== null && hours > 0 && rate !== null && rate > 0) {
                var total = hours * rate;
                $total.val('R ' + total.toFixed(2));
            } else {
                $total.val("");
            }
        }

        $hours.on("input change", updateTotal);
        $rate.on("input change", updateTotal);

        updateTotal();
    });
})(jQuery);
