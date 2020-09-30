<?php
	$file = './OutData/outputs.txt';
	$date = date("D M j G:i:s T Y");

	$timeStim = (float) $_POST["time-stimulus"];
	$trialType = $_POST["trial-type"];
	$timeResp = (float) $_POST["time-response"];
	$respType = $_POST["response"];

	$content = $timeStim . ', ' . $trialType . ', ' . $timeResp . ', ' . $respType . "\n"; 

	file_put_contents($file, $date .= "\n", FILE_APPEND);
	file_put_contents($file, $content , FILE_APPEND);
?>