$("input").blur(function () {
	if ($(this).attr("data-selected-all")) {
		$(this).removeAttr("data-selected-all");
	}
});

$("input").click(function () {
	if (!$(this).attr("data-selected-all")) {
		try {
			$(this).selectionStart = 0;
			$(this).selectionEnd = $(this).value.length + 1;
			//add atribute allowing normal selecting post focus
			$(this).attr("data-selected-all", true);
		} catch (err) {
			$(this).select();
			//add atribute allowing normal selecting post focus
			$(this).attr("data-selected-all", true);
		}
	}
});