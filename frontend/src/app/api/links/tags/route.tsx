import { NextRequest, NextResponse } from 'next/server';
import { ApiV1LinksTagsGetRequest, LinksApi } from '@/apis/LinksApi';

export async function GET(req: NextRequest) {
   const { searchParams } = new URL(req.url);

   // Extract query parameters
   const countParam = searchParams.get('count');
   const tagsParam = searchParams.get('tags');

   const linksApi = new LinksApi();

   // Prepare request for generated API
   const apiRequest: ApiV1LinksTagsGetRequest = {
      count: countParam ? Number(countParam) : 50,
      tags: tagsParam ?? '',
   };

   try {
      const data = await linksApi.apiV1LinksTagsGet(apiRequest);
      return NextResponse.json(data);
   } catch (error) {
      return NextResponse.json(
         { error: (error as Error).message || 'Failed to fetch tags' },
         { status: 500 },
      );
   }
}
