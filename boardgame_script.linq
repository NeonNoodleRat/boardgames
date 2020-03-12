<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\System.Windows.Forms.dll</Reference>
  <Namespace>System.Net</Namespace>
</Query>

public class BoardGame
{
	public string Name {get; set;}	
	public string Published {get; set;}
	public string MinPlayers {get; set;}
	public string MaxPlayers {get; set;}
	public string MinTime {get; set;}
	public string MaxTime {get; set;}
	public string Description {get; set;}
	public string Image {get; set;}
}

const string bbgIds = @"207830,68448,67877,230802,27225,143741,211534";//,10547,206504,1293,200646,27710,118174,39856,172540,102548,260300,1339,207062,255481,202494,112686,172225,172242,258,65244,195205,171668,98778,5451,154597,282524,15393,54043,272738,2448,70323,204583,129622,143884,259081,209778,244992,179275,156336,205322,150658,163412,246639,156976,3076,237728,180198,9851,92415,212765,184491,162886,256606,63268,189182,91534,192291,133473,271869,1111,236709,167791,280501,218530,216439,255708,16992,2223,247342,150312,266192,2243";

void Main()
{
	string[] idList = bbgIds.Replace(" ", "").Split(',');
	//idList.Dump();
	
	int count = 0;
	//string openRow = "<div class='row board-game-row'>";
	string openRow = "";

	foreach(var item in idList)
	{
		
		//string closeRow = "</div>";
		BoardGame game = WebCall(item);
		string gameContainer = FillTemplate(game);
		openRow += gameContainer;
		//count++;
		
//		if (count == 3)
//		{
//			openRow += closeRow;
//			count = 0;
//			
//			Console.WriteLine(openRow);
//			
//			openRow = "<div class='row board-game-row'>";
		//}
		//Console.WriteLine(openRow);
	}
	Console.WriteLine(openRow);
}

public BoardGame WebCall(string gameCode)
{
	BoardGame game = new BoardGame();
	string url = "https://www.boardgamegeek.com/xmlapi/boardgame/" + gameCode;

	// Create a request for the URL.   
	WebRequest request = WebRequest.Create(url);
	// If required by the server, set the credentials.  
	request.Credentials = CredentialCache.DefaultCredentials;

	// Get the response.  
	WebResponse response = request.GetResponse();
	// Display the status.  
	//Console.WriteLine(((HttpWebResponse)response).StatusDescription);

	// Get the stream containing content returned by the server. 
	// The using block ensures the stream is automatically closed. 
	using (Stream dataStream = response.GetResponseStream())
	{
		// Open the stream using a StreamReader for easy access.  
		StreamReader reader = new StreamReader(dataStream);
		string data = reader.ReadToEnd();
		string replacement = Regex.Replace(data, @"&lt;", "<");
		string replacement2 = Regex.Replace(replacement, @"&gt;", ">");
		string replacement3 = Regex.Replace(replacement2, "&amp;shy;", "");

		Regex yearRegex = new Regex("<yearpublished>(.*?)</yearpublished>");
		Regex nameRegex = new Regex("<name primary=.*?>(.*?)</name>");
		Regex minPlayersRegex = new Regex("<minplayers>(.*?)</minplayers>");
		Regex maxPlayersRegex = new Regex("<maxplayers>(.*?)</maxplayers>");
		Regex minTimeRegex = new Regex("<minplaytime>(.*?)</minplaytime>");
		Regex maxTimeRegex = new Regex("<maxplaytime>(.*?)</maxplaytime>");
		Regex descriptionRegex = new Regex("<description>(.*?)</description>");
		Regex imageRegex = new Regex("<image>(.*?)</image>");

		Match yearMatch = yearRegex.Match(replacement3);
		Match nameMatch = nameRegex.Match(replacement3);
		Match minPlayersMatch = minPlayersRegex.Match(replacement3);
		Match maxPlayersMatch = maxPlayersRegex.Match(replacement3);
		Match minTimeMatch = minTimeRegex.Match(replacement3);
		Match maxTimeMatch = maxTimeRegex.Match(replacement3);
		Match descriptionMatch = descriptionRegex.Match(replacement3);
		Match imageMatch = imageRegex.Match(replacement3);
		//Match shortMatch = shortDescRegex.Match(replacement);

		if (yearMatch.Success)
		{
			game.Published = yearMatch.Groups[1].Value;
		}

		if (nameMatch.Success)
		{
			game.Name = nameMatch.Groups[1].Value;
		}
		
		if (minPlayersMatch.Success)
		{
			game.MinPlayers = minPlayersMatch.Groups[1].Value;
		}
		
		if (maxPlayersMatch.Success)
		{
			game.MaxPlayers = maxPlayersMatch.Groups[1].Value;
		}
		
		if (minTimeMatch.Success)
		{
			game.MinTime = minTimeMatch.Groups[1].Value;
		}
		
		if (maxTimeMatch.Success)
		{
			game.MaxTime = maxTimeMatch.Groups[1].Value;
		}
		
		if (descriptionMatch.Success)
		{
			game.Description = descriptionMatch.Groups[1].Value;
		}
		
		if (imageMatch.Success)
		{
			game.Image = imageMatch.Groups[1].Value;
		}
	}

	// Close the response.  
	response.Close();

	return game;
}

public string FillTemplate(BoardGame game)
{
	string players = game.MinPlayers + "-" + game.MaxPlayers;
	string time = game.MinTime + "-" + game.MaxTime;
	
	if (game.MinPlayers == game.MaxPlayers)
	{
		players = game.MinPlayers;
	}
	
	if (game.MinTime == game.MaxTime)
	{
		time = game.MinTime;
	}
	
	return
	//	@"
	//<div class='col-md-4 board-game-container'>
	//    <img src='" + game.Image + @"' height='300' width='300'>
	//    <div>
	//        <strong>Name:</strong>" + game.Name + 
	//    @"</div>
	//    <div class='details-container hide'>
	//        <div><strong>Published:</strong>" + game.Published + @"</div>
	//        <div><strong>Players:</strong>" + players + @"</div>
	//        <div><strong>Time:</strong>" + time + @"</div>
	//        <div>
	//            <strong>Description:</strong> <br/>
	//            " + game.Description + @"
	//        </div>
	//    </div>
	//</div>
	//	";
	@"
<div class='board-game-container flex-item'>
    <img src='" + game.Image + @"' height='300' width='300'>
    <div>
        <strong>Name:</strong>" + game.Name +
	@"</div>
    <div class='details-container hide'>
        <div><strong>Published:</strong>" + game.Published + @"</div>
        <div><strong>Players:</strong>" + players + @"</div>
        <div><strong>Time:</strong>" + time + @"</div>
        <div>
            <strong>Description:</strong> <br/>
            " + game.Description + @"
        </div>
    </div>
</div>
	";
}


//rLPms6x2u&0MViZx









