
function calcular_Printed() {
    importe_total = 0;
    var error = 0;
    $(".print").each(
        function (index, value) {

            var input = $(this);
            var print = eval($(this).val());
            var valorPO = $(".cantPO").parents("tr #po").find("#po").eq(index).val();
            var numPO = parseInt(valorPO);
            var numPrint = parseInt(print);
            var valores;
            importe_total = importe_total + eval($(this).val());

            if (numPrint > numPO) {
                input.css('border', '2px solid #e03f3f');
                var alert = alertify.alert("Message", 'The value of Print is greater than the missing PO or zero.').set('label', 'Aceptar');
                alert.set({ transition: 'zoom' });
                alert.set('modal', false);
                error++;
            } else if (numPrint === 0) {
                error++;
            }
            else {
                input.css('border', '1px solid #cccccc');
            }

        }
    );

    if (error !== 0) {
        $('#guardarBatch').prop("disabled", true);
        $('#modificarBatch').prop("disabled", true);
    } else {
        $('#guardarBatch').prop("disabled", false);
        $('#modificarBatch').prop("disabled", false);
    }

    $("#totalP").val(importe_total);
    CalcularTotal();
    CalcularTotalPNL();
    CalcularTotalBatchPNL();
    calcular_TotalG();
    CalcularTotalBatch();
    calcular_Restantes();
}

function calcular_TotalPiezas(index) {
    importe_total = 0;
    var error = 0;
      $(".cantBox").each(
        function (index, value) {

            var input = $(this);
            var cantidadBox = eval($(this).val());
           // var valorPO = $(".cantPO").parents("tr #po").find("#po").eq(index).val();
            var numPiezas = parseInt($("#l-piezas").val());
            var numCajas = parseInt(cantidadBox);
            var valores;
            importe_total = numCajas * numPiezas;

            /*if (numPrint > numPO || numPrint === 0) {
                input.css('border', '2px solid #e03f3f');
                error++;
            } else {
                input.css('border', '1px solid #cccccc');
            }*/
            

        }
    );
    $(".totalPiezas").val(importe_total);
  
}

function obtTotalMat(index) {
    var error = 0;
    var valorCalidad =  $(".calidad").parent("tr").find("td").eq(index).text();
    var input = $(".totalPiezas");
    var pCalidad = parseInt(valorCalidad);  
    var nombreT = "#pallet" + index + " .totalPiezas";
    var nombreC = "#pallet" + index + " .cantCajas";
    var nombreP = "#pallet" + index + " .cant"; 
    var numCajas = $(nombreC).val(); 
    var numPiezas = $(nombreP).val();
    var tot = numCajas * numPiezas;   
    $(nombreT).val(tot);
    if (tot > pCalidad ) {
        $(nombreT).css('border', '2px solid #e03f3f');
        error++;
        $("#nuevoPallet").prop('disabled', true);
    } else {
        $(nombreT).css('border', '1px solid #cccccc');
        $("#nuevoPallet").prop('disabled', false);
    }
   
}

function obtTotalPiezas() {
    var error = 0;
    var numFilas = $("#tablaTallasPallet tbody>tr").length;
    for (var i = 1; i <= numFilas; i++) {
        var input = $(".totalPiezas");
        var nombreT = "#pallet" + i + " .totalPiezas";
        var nombreC = "#pallet" + i + " .cantBox";
        var nombreP = "#pallet" + i + " .cant";
        /*$('#tablaTallasPallet tr').each(function () {
            var valor = "td" + " #pallet" + i;
            var customerId = $(input).find(nombreT).eq(i).html();
            var kksj = $(nombreP).val();
        });*/
        var valorCalidad = $(".calidad").parent("tr").find("td").eq(i).text();

        var pCalidad = parseInt(valorCalidad);

        /* var nombreC = $(".cantBox").val();"#pallet" + i + " .cantBox";*/
        var nombreP = "#pallet" + i + " .cant";
        var numCajas = $(".cantBox").val(); //$(nombreC).val();
        var numPiezas = $(nombreP).val();
        var tot = numCajas * numPiezas;
        $(nombreT).val(tot);
        if (tot > pCalidad) {
            $(nombreT).css('border', '2px solid #e03f3f');
            error++;
            $("#nuevoPallet").prop('disabled', true);
        } else {
            $(nombreT).css('border', '1px solid #cccccc');
            $("#nuevoPallet").prop('disabled', false);
        }
    }


}

function calcular_MisPrint() {
    importe_total = 0;
    var error = 0;
    $(".mp").each(
        function (index, value) {
            var input = $(this);
            var Misprint = eval($(this).val());
            var valorPO = $(".cantPO").parents("tr #po").find("#po").eq(index).val();
            var numPO = parseInt(valorPO);
            var numMisPrint = parseInt(Misprint);
            var valores;
            importe_total = importe_total + eval($(this).val());
            if (numMisPrint > numPO) {
                input.css('border', '2px solid #e03f3f');
                var alert = alertify.alert("Message", 'The value of MisPrint is greater than the missing PO.').set('label', 'Aceptar');
                alert.set({ transition: 'zoom' });
                alert.set('modal', false);
                error++;
            } else {
                input.css('border', '1px solid #cccccc');
            }
        }
    );

    if (error !== 0) {
        $('#guardarBatch').prop("disabled", true);
        $('#modificarBatch').prop("disabled", true);
    } else {
        $('#guardarBatch').prop("disabled", false);
        $('#modificarBatch').prop("disabled", false);
    }
    $("#totalM").val(importe_total);
    CalcularTotal();
    calcular_TotalG();
    CalcularTotalBatch();
    CalcularTotalPNL();
    CalcularTotalBatchPNL();
    calcular_Restantes();
}

function calcular_Defect() {
    importe_total = 0;
    var error = 0;
    $(".def").each(
        function (index, value) {
            var input = $(this);
            var def = eval($(this).val());
            var valorPO = $(".cantPO").parents("tr #po").find("#po").eq(index).val();
            var numPO = parseInt(valorPO);
            var numDefect = parseInt(def);
            var valores;
            importe_total = importe_total + eval($(this).val());

            if (numDefect > numPO ) {
                input.css('border', '2px solid #e03f3f');
                var alert = alertify.alert("Message", 'The value of Defect is greater than the missing PO.').set('label', 'Aceptar');
                alert.set({
                    transition: 'zoom'
                });
                alert.set('modal', false);
                error++;
            } else {
                input.css('border', '1px solid #cccccc');
            }
        }
    );
    $("#totalD").val(importe_total);
    CalcularTotal();
    calcular_TotalG();
    CalcularTotalBatch();
    CalcularTotalPNL();
    CalcularTotalBatchPNL();
    calcular_Restantes();
}

function calcular_Repair() {
    importe_total = 0;
    var error = 0;
    $(".rep").each(
        function (index, value) {
            var input = $(this);
            var rep = eval($(this).val());
            var valorPO = $(".cantPO").parents("tr #po").find("#po").eq(index).val();
            var numPO = parseInt(valorPO);
            var numRepair = parseInt(rep);
            var valores;
            importe_total = importe_total + eval($(this).val());

            if (numRepair > numPO) {
                input.css('border', '2px solid #e03f3f');
                var alert = alertify.alert("Message", 'The value of Repair is greater than the missing PO.').set('label', 'Aceptar');
                alert.set({
                    transition: 'zoom'
                });
                alert.set('modal', false);
                error++;
            } else {
                input.css('border', '1px solid #cccccc');
            }
        }
    );
    $("#totalR").val(importe_total);
    CalcularTotal();
    calcular_TotalG();
    CalcularTotalBatch();
    CalcularTotalPNL();
    CalcularTotalBatchPNL();
    calcular_Restantes();
}

function calcular_TotalG() {
    importe_total = 0;
    var print = parseInt($("#totalP").val());
    var misPrint = parseInt($("#totalM").val());
    var defecto = parseInt($("#totalD").val());
    var repair = parseInt($("#totalR").val());
    importe_total = print + misPrint + defecto + repair;
    $("#totalF").val(parseInt(importe_total));

}



function CalcularTotalGeneral() {
    var sum = 0;
    $(".total").each(function () {
        sum += parseFloat($(this).text());
    });
    $('#sum').text(sum);
}
function CalcularTotal() {
    var sumQ = [];
    var nColumnas = $("#tablePrint tr:last td").length;
    var index = nColumnas - 2;

    for (var i = 1; i <= index; i++) {
        sumQ[i] = 0;
        var n = 0;

        $('td:nth-child(' + (i + 1) + ')').find(".txt").each(function () {
            var valorPO = $(".cantPO").parents("tr #po").find("#po").eq(i - 1).val();
            var numPO = parseInt(valorPO);
            if (!isNaN(this.value) && this.value.length !== 0) {

                sumQ[i] += parseInt(this.value);

            }
            var valor = sumQ[i];
            var faltante = numPO - valor;
            $(".totalFal").eq(i - 1).val(faltante);


        });

    }


}

function CalcularTotalPNL() {
    var sumQ = [];
    var nColumnas = $("#tablePnl tr:last td").length;
    var index = nColumnas - 2;

    for (var i = 1; i <= index; i++) {
        sumQ[i] = 0;
        var n = 0;

        $('td:nth-child(' + (i + 1) + ')').find(".txt").each(function () {
            var valorPO = $(".cantPO").parents("tr #po").find("#po").eq(i - 1).val();
            var numPO = parseInt(valorPO);
            if (!isNaN(this.value) && this.value.length !== 0) {

                sumQ[i] += parseInt(this.value);

            }
            var valor = sumQ[i];
            var faltante = numPO - valor;
            $(".totalFal").eq(i - 1).val(faltante);


        });

    }


}

function CalcularTotalBatch() {
    var sumQ = [];
    var nColumnas = $("#tablePrint tr:last td").length;
    var index = nColumnas - 2;

    for (var i = 1; i <= index; i++) {
        sumQ[i] = 0;
        var n = 0;

        $('td:nth-child(' + (i + 1) + ')').find(".txt").each(function () {
            var valorPO = $(".cantPO").parents("tr #po").find("#po").eq(i - 1).val();
            var numPO = parseInt(valorPO);
            if (!isNaN(this.value) && this.value.length !== 0) {

                sumQ[i] += parseInt(this.value);

            }
            var valor = sumQ[i];
            var faltante = numPO - valor;
            $(".totalFalt").eq(i - 1).val(valor);


        });

    }


}

function CalcularTotalBatchPNL() {
    var sumQ = [];
    var nColumnas = $("#tablePnl tr:last td").length;
    var index = nColumnas - 2;

    for (var i = 1; i <= index; i++) {
        sumQ[i] = 0;
        var n = 0;

        $('td:nth-child(' + (i + 1) + ')').find(".txt").each(function () {
            var valorPO = $(".cantPO").parents("tr #po").find("#po").eq(i - 1).val();
            var numPO = parseInt(valorPO);
            if (!isNaN(this.value) && this.value.length !== 0) {

                sumQ[i] += parseInt(this.value);

            }
            var valor = sumQ[i];
            var faltante = numPO - valor;
            $(".totalFalt").eq(i - 1).val(valor);


        });

    }


}

function calcular_Restantes() {
    var error = 0;
    $(".totalFal").each(
        function (index, value) {
            var input = $(this);
            var print = eval($(this).val());
            var valorPO = $(".cantPO").parents("tr #po").find("#po").eq(index).val();
            var numPO = parseInt(valorPO);
            var numPrint = parseInt(print);
            var valores;


            if (numPO >= numPrint) {
                input.css('background-color', '#f97878');

            } 

            if (numPrint === 0) {
                input.css('background-color', '#eee');
                
            } 

        }
    );
}