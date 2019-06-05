

//Desplazamiento tabla de tallas de primera calidad

$('.tbodyQtyTall').keydown(function (e) {
	var $table = $(this);
	var $active = $('input:focus,select:focus', $table);
	var $next = null;
	var focusableQuery = 'input:visible,select:visible,textarea:visible';
	var position = parseInt($active.closest('td').index()) + 1;
	console.log('position :', position);
	switch (e.keyCode) {
		case 37: // <Left>
			$next = $active.parent('td').prev().find(focusableQuery);
			break;
		case 38: // <Up>                    
			$next = $active
				.closest('tr')
				.prev()
				.find('td:nth-child(' + position + ')')
				.find(focusableQuery)
				;

			break;
		case 39: // <Right>
			$next = $active.closest('td').next().find(focusableQuery);
			break;
		case 40: // <Down>
			$next = $active
				.closest('tr')
				.next()
				.find('td:nth-child(' + position + ')')
				.find(focusableQuery)
				;
			break;
	}
	if ($next && $next.length) {
		$next.focus();
	}
});

//Desplazamiento para tabla de registro de pallet ht
$('.tbodyHTPallet').keydown(function (e) {
	var $table = $(this);
	var $active = $('input:focus,select:focus', $table);
	var $next = null;
	var focusableQuery = 'input:visible,select:visible,textarea:visible';
	var position = parseInt($active.closest('td').index()) + 1;
	console.log('position :', position);
	switch (e.keyCode) {
		case 37: // <Left>
			$next = $active.parent('td').prev().find(focusableQuery);
			break;
		case 38: // <Up>                    
			$next = $active
				.closest('tr')
				.prev()
				.find('td:nth-child(' + position + ')')
				.find(focusableQuery)
				;

			break;
		case 39: // <Right>
			$next = $active.closest('td').next().find(focusableQuery);
			break;
		case 40: // <Down>
			$next = $active
				.closest('tr')
				.next()
				.find('td:nth-child(' + position + ')')
				.find(focusableQuery)
				;
			break;
	}
	if ($next && $next.length) {
		$next.focus();
	}
});

//Desplazamiento para tabla de registro de assortment
$('.tbodyRegAssortment').keydown(function (e) {
	var $table = $(this);
	var $active = $('input:focus,select:focus', $table);
	var $next = null;
	var focusableQuery = 'input:visible,select:visible,textarea:visible';
	var position = parseInt($active.closest('td').index()) + 1;
	console.log('position :', position);
	switch (e.keyCode) {
		case 37: // <Left>
			$next = $active.parent('td').prev().find(focusableQuery);
			break;
		case 38: // <Up>                    
			$next = $active
				.closest('tr')
				.prev()
				.find('td:nth-child(' + position + ')')
				.find(focusableQuery)
				;

			break;
		case 39: // <Right>
			$next = $active.closest('td').next().find(focusableQuery);
			break;
		case 40: // <Down>
			$next = $active
				.closest('tr')
				.next()
				.find('td:nth-child(' + position + ')')
				.find(focusableQuery)
				;
			break;
	}
	if ($next && $next.length) {
		$next.focus();
	}
});

//Desplazamiento para tabla de registro de packing bulk
$('.packBulkReg').keydown(function (e) {
	var $table = $(this);
	var $active = $('input:focus,select:focus', $table);
	var $next = null;
	var focusableQuery = 'input:visible,select:visible,textarea:visible';
	var position = parseInt($active.closest('td').index()) + 1;
	console.log('position :', position);
	switch (e.keyCode) {
		case 37: // <Left>
			$next = $active.parent('td').prev().find(focusableQuery);
			break;
		case 38: // <Up>                    
			$next = $active
				.closest('tr')
				.prev()
				.find('td:nth-child(' + position + ')')
				.find(focusableQuery)
				;

			break;
		case 39: // <Right>
			$next = $active.closest('td').next().find(focusableQuery);
			break;
		case 40: // <Down>
			$next = $active
				.closest('tr')
				.next()
				.find('td:nth-child(' + position + ')')
				.find(focusableQuery)
				;
			break;
	}
	if ($next && $next.length) {
		$next.focus();
	}
});

$('.packPPKReg').keydown(function (e) {
	var $table = $(this);
	var $active = $('input:focus,select:focus', $table);
	var $next = null;
	var focusableQuery = 'input:visible,select:visible,textarea:visible';
	var position = parseInt($active.closest('td').index()) + 1;
	console.log('position :', position);
	switch (e.keyCode) {
		case 37: // <Left>
			$next = $active.parent('td').prev().find(focusableQuery);
			break;
		case 38: // <Up>                    
			$next = $active
				.closest('tr')
				.prev()
				.find('td:nth-child(' + position + ')')
				.find(focusableQuery)
				;

			break;
		case 39: // <Right>
			$next = $active.closest('td').next().find(focusableQuery);
			break;
		case 40: // <Down>
			$next = $active
				.closest('tr')
				.next()
				.find('td:nth-child(' + position + ')')
				.find(focusableQuery)
				;
			break;
	}
	if ($next && $next.length) {
		$next.focus();
	}
});
//Desplazamiento de registro del tipo de empaque bulk por piezas
$('.tbodyTallaBulkPcs').keydown(function (e) {
	var $table = $(this);
	var $active = $('input:focus,select:focus', $table);
	var $next = null;
	var focusableQuery = 'input:visible,select:visible,textarea:visible';
	var position = parseInt($active.closest('td').index()) + 1;
	console.log('position :', position);
	switch (e.keyCode) {
		case 37: // <Left>
			$next = $active.parent('td').prev().find(focusableQuery);
			break;
		case 38: // <Up>                    
			$next = $active
				.closest('tr')
				.prev()
				.find('td:nth-child(' + position + ')')
				.find(focusableQuery)
				;

			break;
		case 39: // <Right>
			$next = $active.closest('td').next().find(focusableQuery);
			break;
		case 40: // <Down>
			$next = $active
				.closest('tr')
				.next()
				.find('td:nth-child(' + position + ')')
				.find(focusableQuery)
				;
			break;
	}
	if ($next && $next.length) {
		$next.focus();
	}
});
//Desplazamiento de registro del tipo de empaque ppk por ratio
$('.tbodyTallaPPKRatio').keydown(function (e) {
	var $table = $(this);
	var $active = $('input:focus,select:focus', $table);
	var $next = null;
	var focusableQuery = 'input:visible,select:visible,textarea:visible';
	var position = parseInt($active.closest('td').index()) + 1;
	console.log('position :', position);
	switch (e.keyCode) {
		case 37: // <Left>
			$next = $active.parent('td').prev().find(focusableQuery);
			break;
		case 38: // <Up>                    
			$next = $active
				.closest('tr')
				.prev()
				.find('td:nth-child(' + position + ')')
				.find(focusableQuery)
				;

			break;
		case 39: // <Right>
			$next = $active.closest('td').next().find(focusableQuery);
			break;
		case 40: // <Down>
			$next = $active
				.closest('tr')
				.next()
				.find('td:nth-child(' + position + ')')
				.find(focusableQuery)
				;
			break;
	}
	if ($next && $next.length) {
		$next.focus();
	}
});



